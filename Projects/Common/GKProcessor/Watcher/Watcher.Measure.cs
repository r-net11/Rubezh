using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

namespace GKProcessor
{
	public partial class Watcher
	{
		List<MeasureDeviceInfo> MeasureDeviceInfos = new List<MeasureDeviceInfo>();

		public void StartDeviceMeasure(GKDevice device)
		{
			if (device.Driver.MeasureParameters.Count > 0)
			{
				var measureDeviceInfo = MeasureDeviceInfos.FirstOrDefault(x => x.Device.UID == device.UID);
				if (measureDeviceInfo == null)
				{
					measureDeviceInfo = new MeasureDeviceInfo(device);
					MeasureDeviceInfos.Add(measureDeviceInfo);
				}
				else
				{
					measureDeviceInfo.DateTime = DateTime.Now;
				}
			}
		}

		public void StopDeviceMeasure(GKDevice device)
		{
			MeasureDeviceInfos.RemoveAll(x => x.Device.UID == device.UID);
		}

		void CheckMeasure()
		{
			MeasureDeviceInfos.RemoveAll(x => (DateTime.Now - x.DateTime).TotalSeconds > 120);
			foreach (var measureDeviceInfo in MeasureDeviceInfos)
			{
				List<GKMeasureParameterValue> measureParameters = null;
				if (measureDeviceInfo.Device.KauDatabaseParent != null && measureDeviceInfo.Device.KauDatabaseParent.DriverType == GKDriverType.KAU)
				{
					measureParameters = measureDeviceInfo.GetRSR1Measure();
				}
				else if (measureDeviceInfo.Device.KauDatabaseParent != null && measureDeviceInfo.Device.KauDatabaseParent.DriverType == GKDriverType.RSR2_KAU)
				{
					measureParameters = measureDeviceInfo.GetRSR2Measure();
				}

				//if (measureParameters != null && measureParameters.Count > 0)
				if (measureParameters != null)
				{
					var deviceMeasureParameters = new GKDeviceMeasureParameters();
					deviceMeasureParameters.DeviceUID = measureDeviceInfo.Device.UID;
					foreach (var measureParameter in measureParameters)
					{
						deviceMeasureParameters.MeasureParameterValues.Add(measureParameter);
					}
					OnMeasureParametersChanged(deviceMeasureParameters);
				}
			}
		}
	}

	class MeasureDeviceInfo
	{
		public GKDevice Device { get; private set; }
		public DateTime DateTime { get; set; }
		public List<GKMeasureParameterValue> MeasureParameters;

		int ParameterIndex;
		bool CanMoveToNextParameter;
		int GetParameterTryIndex;
		public DateTime StartTryDateTime { get; set; }

		public MeasureDeviceInfo(GKDevice device)
		{
			Device = device;
			DateTime = DateTime.Now;

			MeasureParameters = new List<GKMeasureParameterValue>();
			foreach (var measureParameter in Device.Driver.MeasureParameters)
			{
				var measureParameterValue = new GKMeasureParameterValue()
				{
					Name = measureParameter.Name
				};
				MeasureParameters.Add(measureParameterValue);
			}
		}

		public List<GKMeasureParameterValue> GetRSR1Measure()
		{
			var measureParameter = Device.Driver.MeasureParameters[ParameterIndex];
			if (CanMoveToNextParameter)
			{
				CanMoveToNextParameter = false;

				ParameterIndex++;
				if (ParameterIndex == Device.Driver.MeasureParameters.Count)
				{
					ParameterIndex = 0;
				}

				StartTryDateTime = DateTime.Now;
				GetParameterTryIndex = 0;
			}

			var bytes = new List<byte>();
			bytes.Add((byte)Device.Driver.DriverTypeNo);
			bytes.Add(Device.IntAddress);
			bytes.Add((byte)(Device.ShleifNo - 1));
			bytes.Add(measureParameter.No);
			var result = SendManager.Send(Device.KauDatabaseParent, 4, 131, 2, bytes);

			if (!result.HasError)
			{
				result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(Device.GKDescriptorNo));
				if (!result.HasError && result.Bytes.Count > 0)
				{
					var resievedParameterNo = result.Bytes[63];
					if (resievedParameterNo == measureParameter.No)
					{
						var parameterUshortValue = (short)BytesHelper.SubstructShort(result.Bytes, 64);
						var measureParameterValue = ParceRSR1MeasureParameter(measureParameter, parameterUshortValue);
						CanMoveToNextParameter = true;
						return new List<GKMeasureParameterValue>() { measureParameterValue };
					}
				}
			}

			GetParameterTryIndex++;
			if (GetParameterTryIndex > 10 && (DateTime.Now - StartTryDateTime).TotalSeconds > 60)
			{
				CanMoveToNextParameter = true;
			}
			return null;
		}

		GKMeasureParameterValue ParceRSR1MeasureParameter(GKMeasureParameter measureParameter, short parameterShortValue)
		{
			if (measureParameter.IsHighByte)
			{
				parameterShortValue = (short)(parameterShortValue / 256);
			}
			else if (measureParameter.IsLowByte)
			{
				parameterShortValue = (short)(parameterShortValue << 8);
				parameterShortValue = (short)(parameterShortValue >> 8);
			}
			double parameterValue;
			if (measureParameter.Multiplier != null)
				parameterValue = (double)parameterShortValue / (double)measureParameter.Multiplier;
			else
				parameterValue = parameterShortValue;
			var stringValue = parameterValue.ToString();
			if (measureParameter.Name == "Дата последнего обслуживания, м.г.")
			{
				stringValue = (parameterShortValue % 256).ToString() + "." + (parameterShortValue / 256).ToString();
			}
			if ((Device.DriverType == GKDriverType.Valve || Device.Driver.IsPump) && measureParameter.Name == "Режим работы")
			{
				stringValue = "Неизвестно";
				switch (parameterShortValue & 3)
				{
					case 0:
						stringValue = "Автоматический";
						break;

					case 1:
						stringValue = "Ручной";
						break;

					case 2:
						stringValue = "Отключено";
						break;
				}
			}

			var measureParameterValue = MeasureParameters.FirstOrDefault(x => x.Name == measureParameter.Name);
			measureParameterValue.StringValue = stringValue;
			return measureParameterValue;
		}

		public List<GKMeasureParameterValue> GetRSR2Measure()
		{
			var result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(Device.GKDescriptorNo));
			if (!result.HasError && result.Bytes.Count > 0)
			{
				if (Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV)
				{
					ParceRSR2Valve(result.Bytes);
				}
				else
				{
					foreach (var measureParameter in Device.Driver.MeasureParameters)
					{
						var parameterValue = BytesHelper.SubstructShort(result.Bytes, 44 + measureParameter.No * 2);

						var stringValue = parameterValue.ToString();
						if (measureParameter.Multiplier != null)
						{
							var doubleValue = (double)parameterValue / (double)measureParameter.Multiplier;
							stringValue = doubleValue.ToString();
						}

						if (measureParameter.Name == "Дата последнего обслуживания, м.г.")
						{
							stringValue = (parameterValue % 256).ToString() + "." + (parameterValue / 256).ToString();
						}

						var measureParameterValue = MeasureParameters.FirstOrDefault(x => x.Name == measureParameter.Name);
						measureParameterValue.StringValue = stringValue;
					}
				}
				return MeasureParameters;
			}
			return null;
		}

		void ParceRSR2Valve(List<byte> bytes)
		{
			MeasureParameters = new List<GKMeasureParameterValue>();
			var failureParameterValue = BytesHelper.SubstructShort(bytes, 44 + 1 * 2);

			if (IsBitSet(failureParameterValue, 15))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Недопустимое сочетание сигналов", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 14))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Отказ задвижки(зклинило)", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 13))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Питание контроллера", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 12))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Обрыв цепи ПД", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 11))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Авария питания(питание силовое)", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 10))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Контактор закр", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 9))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Контактор откр", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 8))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Вскрытие", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 7))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "ДУстоп", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 6))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "ДУзакр", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 5))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "ДУоткр", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 4))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "ОГВ", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 3))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "МВзакр(ДВУ)", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 2))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "МВОткр(ДНУ)", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 1))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "КВЗакр", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 0))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "КВОткр", StringValue = "Да" });
			}

			var delayParameterValue = BytesHelper.SubstructShort(bytes, 44 + 2 * 2);
			var delayTypeParameterValue = BytesHelper.SubstructShort(bytes, 44 + 3 * 2);
			if (delayTypeParameterValue == 1)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Задержка, с", StringValue = delayParameterValue.ToString() });
			}
			if (delayTypeParameterValue == 2)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Время хода, с", StringValue = delayParameterValue.ToString() });
			}
			if (delayTypeParameterValue == 3)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Удержание, с", StringValue = delayParameterValue.ToString() });
			}

			var valveTypeParameterValue = BytesHelper.SubstructShort(bytes, 44 + 4 * 2);
			if (delayTypeParameterValue == 1)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "Тип 1" });
			}
			if (delayTypeParameterValue == 2)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "Тип 2" });
			}
			if (delayTypeParameterValue == 3)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "Тип 3" });
			}

			var failureDescriptionParameterValue = BytesHelper.SubstructShort(bytes, 44 + 5 * 2);
			if (IsBitSet(failureDescriptionParameterValue, 9))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Есть недопустимое сочетание у МВоткр", StringValue = "Да" });
			}
			if (IsBitSet(failureDescriptionParameterValue, 8))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Есть недопустимое сочетание у КВоткр", StringValue = "Да" });
			}
		}

		bool IsBitSet(ushort value, int position)
		{
			return (value & (1 << position)) != 0;
		}
	}
}