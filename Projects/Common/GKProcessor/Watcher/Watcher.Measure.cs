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

				if (measureParameters != null && measureParameters.Count > 0)
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
						var measureParameterValue = ParceRSRaMeasureParameter(measureParameter, parameterUshortValue);
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

		GKMeasureParameterValue ParceRSRaMeasureParameter(GKMeasureParameter measureParameter, short parameterShortValue)
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
				parameterValue = parameterShortValue / (double)measureParameter.Multiplier;
			else
				parameterValue = parameterShortValue;
			var stringValue = parameterValue.ToString();
			if (measureParameter.Name == "Дата последнего обслуживания, м.г.")
			{
				stringValue = (parameterShortValue % 256).ToString() + "." + (parameterShortValue / 256).ToString();
			}
			if ((Device.DriverType == GKDriverType.Valve || Device.Driver.IsPump)
				&& measureParameter.Name == "Режим работы")
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
			measureParameterValue.Value = parameterValue;
			measureParameterValue.StringValue = stringValue;
			return measureParameterValue;
		}

		public List<GKMeasureParameterValue> GetRSR2Measure()
		{
			var result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(Device.GKDescriptorNo));
			if (!result.HasError && result.Bytes.Count > 0)
			{
				for (int i = 0; i < Device.Driver.MeasureParameters.Count; i++)
				{
					var measureParameter = Device.Driver.MeasureParameters[i];
					var parameterValue = BytesHelper.SubstructShort(result.Bytes, 48 + i * 2);
					var stringValue = parameterValue.ToString();
					if (measureParameter.Name == "Дата последнего обслуживания, м.г.")
					{
						stringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
					}

					var measureParameterValue = MeasureParameters.FirstOrDefault(x => x.Name == measureParameter.Name);
					measureParameterValue.Value = parameterValue;
					measureParameterValue.StringValue = stringValue;
				}
				return MeasureParameters;
			}
			return null;
		}
	}
}