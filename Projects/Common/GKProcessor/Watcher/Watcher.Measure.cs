using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using RubezhClient;
using RubezhDAL;

namespace GKProcessor
{
	public partial class Watcher
	{
		List<MeasureDeviceInfo> MeasureDeviceInfos = new List<MeasureDeviceInfo>();

		public void StartDeviceMeasure(GKDevice device)
		{
			if (device.Driver.MeasureParameters.Count > 0 || device.DriverType == GKDriverType.RSR2_Valve_DU || device.DriverType == GKDriverType.RSR2_Valve_KV || device.DriverType == GKDriverType.RSR2_Valve_KVMV)
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
				if (measureDeviceInfo.Device.KauDatabaseParent != null && measureDeviceInfo.Device.KauDatabaseParent.DriverType == GKDriverType.RSR2_KAU)
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

		void CheckKAUMeasure()
		{
			foreach (var device in GkDatabase.RootDevice.Children.Where(x => x.DriverType == GKDriverType.RSR2_KAU))
			{
				using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
				{
					skdDatabaseService.CurrentConsumptionTranslator.SaveMany(GetKAUMeasure(device));
				}
			}
		}

		public static List<CurrentConsumption> GetKAUMeasure(GKDevice device, int? alsIntAddress = null)
		{
			var result = SendManager.Send(device.GkDatabaseParent, 2, 20, 16, BytesHelper.ShortToBytes(device.GKDescriptorNo));
			var alsCurrents = new List<CurrentConsumption>();
			if (!result.HasError && result.Bytes.Count == 16)
			{
				foreach (var alsDevice in device.Children.Where(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif && (alsIntAddress == null || x.IntAddress == alsIntAddress)))
				{
					var current = (double)BytesHelper.SubstructShort(result.Bytes, (alsDevice.IntAddress - 1) * 2) * 300 / 4096;
					var alsCurrent = new CurrentConsumption();
					alsCurrent.AlsUID = alsDevice.UID;
					alsCurrent.Current = (int)current;
					alsCurrent.DateTime = DateTime.Now;
					alsCurrents.Add(alsCurrent);
				}
			}
			return alsCurrents;
		}
	}

	class MeasureDeviceInfo
	{
		public GKDevice Device { get; private set; }
		public DateTime DateTime { get; set; }
		public List<GKMeasureParameterValue> MeasureParameters;

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
						string stringValue = "";
						ushort parameterValue = BytesHelper.SubstructShort(result.Bytes, 46 + measureParameter.No * 2);
						if (measureParameter.HasNegativeValue)
						{
							stringValue = ((short)parameterValue).ToString();
						}
						else
						{
							stringValue = parameterValue.ToString();
						}
						if (measureParameter.Multiplier != null)
						{
							var doubleValue = Math.Round((parameterValue / (double)measureParameter.Multiplier), 3);
							stringValue = doubleValue.ToString();
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
			var failureParameterValue = BytesHelper.SubstructShort(bytes, 46 + 6 * 2);
			var failureDescriptionParameterValue = BytesHelper.SubstructShort(bytes, 46 + 4 * 2);
			var stateParameterValue = BytesHelper.SubstructShort(bytes, 46 + 5 * 2);

			if (IsBitSet(failureParameterValue, 15))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Недопустимое сочетание сигналов КВ и МВ", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 14))
			{
				var stringValue = "";
				if (IsBitSet(failureDescriptionParameterValue, 14))
				{
					stringValue = "МВ без КВ";
				}
				else
				{
					stringValue = "Истекло время хода";
				}
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Отказ задвижки(заклинило)", StringValue = stringValue });
			}
			if (IsBitSet(failureParameterValue, 13))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Авария питания контроллера", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 12))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Обрыв цепи питания двигателя", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 11))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Авария силового питания", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 10))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Контактор закрыть", StringValue = "Да" });
			}
			if (IsBitSet(failureParameterValue, 9))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Контактор открыть", StringValue = "Да" });
			}

			//if (IsBitSet(failureParameterValue, 8))
			//{
			//	MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Вскрытие", StringValue = "Да" });
			//}

			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 7, "ДУ кнопка стоп");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 6, "ДУ кнопка закрыть");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 5, "ДУ кнопка открыть");
			//SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 4, "ОГВ");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 3, "МВ закрыто");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 2, "МВ открыто");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 1, "КВ закрыто");
			SetValveParameter(failureParameterValue, failureDescriptionParameterValue, stateParameterValue, 0, "КВ открыто");

			MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Управление с ГК", StringValue = IsBitSet(stateParameterValue, 13) ? "Р" : "А" });

			//if (IsBitSet(stateParameterValue, 8))
			//{
			//	MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Отсчет задержки на открытие", StringValue = "Да" });
			//}
			//if (IsBitSet(stateParameterValue, 12))
			//{
			//	MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Ход на закрытие", StringValue = "Да" });
			//}
			//if (IsBitSet(stateParameterValue, 11))
			//{
			//	MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Ход на открытие", StringValue = "Да" });
			//}

			var delayParameterValue = BytesHelper.SubstructShort(bytes, 46 + 1 * 2);
			var delayTypeParameterValue = BytesHelper.SubstructShort(bytes, 46 + 2 * 2);
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

			var valveTypeParameterValue = BytesHelper.SubstructShort(bytes, 46 + 3 * 2);
			if (valveTypeParameterValue == 1)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "КВ" });
			}
			if (valveTypeParameterValue == 2)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "КВ и МВ" });
			}
			if (valveTypeParameterValue == 3)
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = "Тип задвижки", StringValue = "ДУ" });
			}
		}

		void SetValveParameter(ushort failureParameterValue, ushort failureDescriptionParameterValue, ushort stateParameterValue, int bitPosition, string name)
		{
			if (IsBitSet(failureParameterValue, bitPosition))
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = name, StringValue = IsBitSet(failureDescriptionParameterValue, bitPosition) ? "КЗ" : "Обрыв" });
			}
			else
			{
				MeasureParameters.Add(new GKMeasureParameterValue() { Name = name, StringValue = IsBitSet(stateParameterValue, bitPosition) ? "Есть" : "Нет сигнала" });
			}
		}

		bool IsBitSet(ushort value, int position)
		{
			return (value & (1 << position)) != 0;
		}
	}
}