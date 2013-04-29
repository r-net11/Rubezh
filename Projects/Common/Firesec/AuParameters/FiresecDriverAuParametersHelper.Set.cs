using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace Firesec
{
	public static partial class FiresecDriverAuParametersHelper
	{
		public static void SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var binProperties = new List<BinProperty>();
			foreach (var property in properties)
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
						if (driverProperty.Caption == "Проигрываемое сообщение")
						{
							intValue = MRO2Helper.SetMessageNumber(intValue);
						}
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