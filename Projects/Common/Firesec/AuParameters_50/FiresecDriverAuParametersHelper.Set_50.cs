using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using System.Threading;

namespace Firesec_50
{
	public static partial class FiresecDriverAuParametersHelper
	{
		public static void BeginSetAuParameters(List<Device> devices)
		{
			StopAUParametersThread();
			StopEvent = new AutoResetEvent(false);
			AUParametersThread = new Thread(() => { SetAuParameters(devices); });
			AUParametersThread.Start();
		}

		static void SetAuParameters(List<Device> devices)
		{
			for (int i = 0; i < devices.Count; i++)
			{
				var device = devices[i];
				OnPropgress("Запись параметров устройства " + device.DottedPresentationNameAndAddress, (i * 100) / devices.Count);
				var binProperties = new List<BinProperty>();
				foreach (var property in device.SystemAUProperties)
				{
					var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (driverProperty != null && driverProperty.IsAUParameter && !driverProperty.IsReadOnly)
					{
						var binProperty = binProperties.FirstOrDefault(x => x.No == driverProperty.No);

						if (binProperty == null)
						{
							binProperty = new BinProperty()
							{
								No = driverProperty.No
							};
							binProperties.Add(binProperty);
						}

						int intValue = 0;
						if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.EnumType)
						{
							var driverPropertyParameterValue = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
							if (driverPropertyParameterValue != null)
							{
								intValue = int.Parse(driverPropertyParameterValue.Value);
							}
						}
						else if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.BoolType)
						{
							if (property.Value == null)
							{
								intValue = 0;
							}
							else
							{
								intValue = 1;
							}
						}
						else
						{
							intValue = int.Parse(property.Value);
							if (driverProperty.Caption == "Задержка включения МРО, с")
							{
								intValue = (int)Math.Truncate((double)intValue / 5);
							}
						}

						if (driverProperty.Multiplier > 0)
						{
							intValue = (int)(intValue * driverProperty.Multiplier);
						}

						if (driverProperty.BitOffset > 0)
						{
							intValue = intValue << driverProperty.BitOffset;
						}

						if (driverProperty.UseMask)
						{
							binProperty.HighByte += intValue;
							binProperty.LowByte = 0xFF;
						}
						else if (driverProperty.HighByte)
							binProperty.LowByte += intValue;
						else if (driverProperty.LargeValue)
						{
							var HighVal = intValue / 256;
							var LowVal = intValue - HighVal * 256;
							binProperty.LowByte = HighVal;
							binProperty.HighByte = LowVal;
						}
						else
							binProperty.HighByte += intValue;
					}
				}

				foreach (var binProperty in binProperties)
				{
					int requestId = 0;
					FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$WriteSimpleParam", binProperty.ToString(), ref requestId);
				}
			}
			AUParametersThread = null;
			OnPropgress("Готово", 0);
		}

		static int ExchangeLowAndHighBytes(int value)
		{
			return value / 256 + (value - (value / 256) * 256) * 256;
		}

		class BinProperty
		{
			public int No;
			public int LowByte;
			public int HighByte;
			public override string ToString()
			{
				var value = (byte)LowByte + (byte)HighByte * 256;
				return No.ToString() + "=" + value.ToString();
			}
		}
	}
}