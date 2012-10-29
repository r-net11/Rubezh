using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using Common;

namespace Firesec
{
	public partial class FiresecDriver
	{
		public  OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			var properties = new List<Property>();
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);

			var propertyNos = new HashSet<int>();
			foreach (var property in device.Driver.Properties)
			{
				if (property.IsAUParameter)
				{
					propertyNos.Add(property.No);
				}
			}

			int requestId = 0;
			var requestIds = new List<int>();
			foreach (var propertyNo in propertyNos)
			{
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$ReadSimpleParam", propertyNo.ToString(), ref requestId);
				Trace.WriteLine("ReadSimpleParam RequestId: " + requestId.ToString());
				requestIds.Add(requestId);
			}

			int count = propertyNos.Count;
			
			while (count > 0)
			{
				var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "StateConfigQueries", null, ref requestId);
				//Trace.WriteLine("StateConfigQueries RequestId: " + requestId.ToString());
				//Trace.WriteLine("GetConfigurationParameters: " + result.Result);

				if (result.HasError)
				{
					return new OperationResult<List<Property>>()
					{
						Result = null,
						HasError = true,
						Error = result.Error
					};
				}

				Firesec.Models.DeviceCustomFunctions.requests requests = SerializerHelper.Deserialize<Firesec.Models.DeviceCustomFunctions.requests>(result.Result);

				if (requests != null && requests.request.Count() > 0)
				{
                    //if (requestIds.Contains(requests.request.First().id))
                    {
                        
						int address = requests.request.First().param.FirstOrDefault(x => x.name == "ParamNo").value;
						int fullvalue = requests.request.First().param.FirstOrDefault(x => x.name == "ParamValue").value;
						if (fullvalue != -1)
							Trace.WriteLine(address.ToString() + " " + fullvalue.ToString());
						count--;
                        foreach (var driverProperty in device.Driver.Properties.FindAll(x => x.No == address))
                        {
						    if (properties.FirstOrDefault(x => x.Name == driverProperty.Name) == null)
                            {
                                properties.Add(CreateProperty(fullvalue, driverProperty));
                            }
						}
                    }
					//else
					//{
					//    Logger.Error("FiresecDriver.GetConfigurationParameters RequestIds.Contains = false");
					//}
				}
					
				int waitCount = 0;
				Thread.Sleep(TimeSpan.FromSeconds(1));
				waitCount++;
				if (waitCount > 120)
				{
					return new OperationResult<List<Property>>()
					{
						Result = null,
						HasError = true,
						Error = "Превышено время выполнения запроса в 2 минуты"
					};
				}
			}

			return new OperationResult<List<Property>>()
			{
				Result = properties
			};
		}

		private static Property CreateProperty(int paramValue, DriverProperty driverProperty)
		{
			var offsetParamValue = paramValue;

			var highByteValue = paramValue / 256;
			var lowByteValue = paramValue - highByteValue * 256;

			if (driverProperty.HighByte)
				offsetParamValue = highByteValue;
			else if (driverProperty.LargeValue)
				offsetParamValue = paramValue;
			else
				offsetParamValue = lowByteValue;

			if (driverProperty.MinBit > 0)
			{
				byte byteOffsetParamValue = (byte)offsetParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> driverProperty.MinBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue << driverProperty.MinBit);
				offsetParamValue = byteOffsetParamValue;
			}

			if (driverProperty.MaxBit > 0)
			{
				byte byteOffsetParamValue = (byte)offsetParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - driverProperty.MaxBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - driverProperty.MaxBit);
				offsetParamValue = byteOffsetParamValue;
			}

			if (driverProperty.BitOffset > 0)
			{
				offsetParamValue = offsetParamValue >> driverProperty.BitOffset;
			}

			if (driverProperty.Name == "Задержка включения МРО, сек")
			{
				offsetParamValue = offsetParamValue * 5;
			}

			var property = new Property()
			{
				Name = driverProperty.Name,
				Value = offsetParamValue.ToString()
			};

			return property;
		}

		public void SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var binProperties = new List<BinProperty>();

			foreach (var property in properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				
				if (driverProperty != null && driverProperty.IsAUParameter)
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
					if (binProperty.No == 0x8e
						//&& binProperty.No <= 0xbf
					)
					{
						;
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
						if (driverProperty.Name == "Задержка включения МРО, сек")
						{
							intValue = (int)Math.Truncate((double)intValue / 5);
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
						var LowVal = intValue - HighVal*256;
						binProperty.LowByte = HighVal;
						binProperty.HighByte = LowVal;
					}
					else
						binProperty.HighByte += intValue;
					
				}
			}

			foreach (var binProperty in binProperties)
			{
				if (binProperty.No == 0x8c
					//&& binProperty.No <= 0xbf
					)
				{
					;
				}
				Trace.WriteLine(binProperty.ToString());
				int requestId = 0;
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$WriteSimpleParam", binProperty.ToString(), ref requestId);
				//Trace.WriteLine(binProperty.ToString());
			}
		}

		int ExchengeLowAndHigtBytes(int value)
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