using System.Collections.Generic;
using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecClient;
using Infrastructure.Common.Services;

namespace GKProcessor
{
	public partial class Watcher
	{
		List<MeasureDeviceInfo> MeasureDeviceInfos = new List<MeasureDeviceInfo>();

		public void StartDeviceMeasure(XDevice device)
		{
			if (device.Driver.AUParameters.Count > 0)
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

		public void StopDeviceMeasure(XDevice device)
		{
			MeasureDeviceInfos.RemoveAll(x => x.Device.UID == device.UID);
		}

		void CheckMeasure()
		{
			MeasureDeviceInfos.RemoveAll(x => (DateTime.Now - x.DateTime).TotalSeconds > 120);
			foreach (var measureDeviceInfo in MeasureDeviceInfos)
			{
				List<XMeasureParameter> measureParameters = null;
				if (measureDeviceInfo.Device.KauDatabaseParent != null && measureDeviceInfo.Device.KauDatabaseParent.DriverType == XDriverType.KAU)
				{
					measureParameters = measureDeviceInfo.GetRSR1Measure();
				}
				else if (measureDeviceInfo.Device.KauDatabaseParent != null && measureDeviceInfo.Device.KauDatabaseParent.DriverType == XDriverType.RSR2_KAU)
				{
					measureParameters = measureDeviceInfo.GetRSR2Measure();
				}

				if (measureParameters != null && measureParameters.Count > 0)
				{
					var deviceMeasureParameters = new XDeviceMeasureParameters();
					deviceMeasureParameters.DeviceUID = measureDeviceInfo.Device.UID;
					foreach (var auParameterValue in measureParameters)
					{
						deviceMeasureParameters.MeasureParameters.Add(auParameterValue);
					}
					OnMeasureParametersChanged(deviceMeasureParameters);
				}
			}
		}
	}

	class MeasureDeviceInfo
	{
		public XDevice Device { get; private set; }
		public DateTime DateTime { get; set; }
		public List<XMeasureParameter> XMeasureParameters;

		int ParameterIndex;
		bool CanMoveToNextParameter;
		bool StartGetCommandSucseeded;
		int GetParameterTryIndex;
		public DateTime StartTryDateTime { get; set; }

		public MeasureDeviceInfo(XDevice device)
		{
			Device = device;
			DateTime = DateTime.Now;

			XMeasureParameters = new List<XMeasureParameter>();
			foreach (var auParameter in Device.Driver.AUParameters)
			{
				var auParameterValue = new XMeasureParameter()
				{
					Name = auParameter.Name
				};
				XMeasureParameters.Add(auParameterValue);
			}
		}

		public List<XMeasureParameter> GetRSR1Measure()
		{
			var auParameter = Device.Driver.AUParameters[ParameterIndex];
			if (CanMoveToNextParameter)
			{
				CanMoveToNextParameter = false;
				StartGetCommandSucseeded = false;

				ParameterIndex++;
				if (ParameterIndex == Device.Driver.AUParameters.Count)
				{
					ParameterIndex = 0;
				}
			}

			if (!StartGetCommandSucseeded)
			{
				var bytes = new List<byte>();
				bytes.Add((byte)Device.Driver.DriverTypeNo);
				bytes.Add(Device.IntAddress);
				bytes.Add((byte)(Device.ShleifNo - 1));
				bytes.Add(auParameter.No);
				var result1 = SendManager.Send(Device.KauDatabaseParent, 4, 131, 2, bytes);

				StartGetCommandSucseeded = !result1.HasError;
				if (StartGetCommandSucseeded)
				{
					StartTryDateTime = DateTime.Now;
					GetParameterTryIndex = 0;
				}
				return null;
			}

			var result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(Device.GKDescriptorNo));
			if (!result.HasError && result.Bytes.Count > 0)
			{
				var resievedParameterNo = result.Bytes[63];
				if (resievedParameterNo == auParameter.No)
				{
					var parameterUshortValue = BytesHelper.SubstructShort(result.Bytes, 64);
					if (auParameter.IsHighByte)
					{
						parameterUshortValue = (ushort)(parameterUshortValue / 256);
					}
					else if (auParameter.IsLowByte)
					{
						parameterUshortValue = (ushort)(parameterUshortValue << 8);
						parameterUshortValue = (ushort)(parameterUshortValue >> 8);
					}
					double parameterValue;
					if (auParameter.Multiplier != null)
						parameterValue = parameterUshortValue / (double)auParameter.Multiplier;
					else
						parameterValue = parameterUshortValue;
					var stringValue = parameterValue.ToString();
					if (auParameter.Name == "Дата последнего обслуживания")
					{
						stringValue = (parameterUshortValue / 256).ToString() + "." + (parameterUshortValue % 256).ToString();
					}
					if ((Device.DriverType == XDriverType.Valve || Device.DriverType == XDriverType.Pump)
						&& auParameter.Name == "Режим работы")
					{
						stringValue = "Неизвестно";
						switch (parameterUshortValue & 3)
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

					var auParameterValue = XMeasureParameters.FirstOrDefault(x => x.Name == auParameter.Name);
					auParameterValue.Value = parameterValue;
					auParameterValue.StringValue = stringValue;

					CanMoveToNextParameter = true;
					return new List<XMeasureParameter>() { auParameterValue };
				}
			}

			GetParameterTryIndex++;
			if (GetParameterTryIndex > 10 && (DateTime.Now - StartTryDateTime).TotalSeconds > 60)
			{
				CanMoveToNextParameter = true;
			}
			return null;
		}

		public List<XMeasureParameter> GetRSR2Measure()
		{
			var result = SendManager.Send(Device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(Device.GKDescriptorNo));
			if (!result.HasError && result.Bytes.Count > 0)
			{
				for (int i = 0; i < Device.Driver.AUParameters.Count; i++)
				{
					var auParameter = Device.Driver.AUParameters[i];
					var parameterValue = BytesHelper.SubstructShort(result.Bytes, 48 + i * 2);
					var stringValue = parameterValue.ToString();
					if (auParameter.Name == "Дата последнего обслуживания")
					{
						stringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
					}

					var auParameterValue = XMeasureParameters.FirstOrDefault(x => x.Name == auParameter.Name);
					auParameterValue.Value = parameterValue;
					auParameterValue.StringValue = stringValue;
				}
				return XMeasureParameters;
			}
			return null;
		}
	}
}