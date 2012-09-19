using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public partial class FiresecDriver
	{
		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			var properties = new List<Property>();
			var requestIds = new List<int>();
			int requestId = 0;
			int waitCount = 0;
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsAUParameter)
				{
					FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$ReadSimpleParam", property.No.ToString(), ref requestId);
					requestIds.Add(requestId);
				}
			}

			while (true)
			{
				var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "StateConfigQueries", null, ref requestId);
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
				if (requests != null)
				{
					foreach (var request in requests.request)
					{
						if (requestIds.Contains(request.id))
						{
							requestIds.Remove(request.id);
							var paramNo = request.param.FirstOrDefault(x => x.name == "ParamNo").value;
							var paramValue = request.param.FirstOrDefault(x => x.name == "ParamValue").value;
							
							var heightByteValue = paramValue / 256;
							var lowByteValue = paramValue - heightByteValue * 256;

							foreach (var driverProperty in device.Driver.Properties)
							{
								if (driverProperty.No == paramNo)
								{
									if (paramNo == 0x8d && driverProperty.Name == "датчик уровня"
										//&& paramNo <= 0xbf
										)
									{
										;
									}
									var offsetParamValue = paramValue;

									if (driverProperty.HighByte)
										offsetParamValue = heightByteValue;
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
									
									if (device.Driver.DriverType == DriverType.MRO && driverProperty.Name == "Время отложенного пуска")
									{
										offsetParamValue = offsetParamValue * 5;
									}

									var property = new Property()
									{
										Name = driverProperty.Name,
										Value = offsetParamValue.ToString()
									};
									if (properties.FirstOrDefault(x => x.Name == property.Name) == null)
										properties.Add(property);
								}
							}
						}
					}
				}
				if (requestIds.Count == 0)
				{
					break;
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
				waitCount++;
				if (waitCount > 600)
				{
					return new OperationResult<List<Property>>()
					{
						Result = null,
						HasError = true,
						Error = "Превышено время выполнения запроса"
					};
				}
			}

			return new OperationResult<List<Property>>()
			{
				Result = properties
			};
		}

		public void SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var binProperties = new List<BinProperty>();

			foreach (var property in properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				//if (driverProperty.No == 0x8c
				//    //&& binProperty.No <= 0xbf
				//        )
				//{
				//    ;
				//}
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

					int intValue = 0;
					if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.EnumType)
					{
						var driverPropertyParameterValue = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
						if (driverPropertyParameterValue != null)
						{
							intValue = int.Parse(driverPropertyParameterValue.Value);
						}
					}
					else
					{
						intValue = int.Parse(property.Value);
						if (device.Driver.DriverType == DriverType.MRO && driverProperty.Name == "Время отложенного пуска")
						{
							intValue = (int)Math.Truncate((double)intValue / 5);
						}
					}
					//if (intValue < driverProperty.Min || intValue > driverProperty.Max)
					//{
					//    MessageBox.Show("Значение параметра " + driverProperty.Caption + " вне допустимого диапазона");
					//    return ;
					//}

					if (driverProperty.BitOffset > 0)
					{
						intValue = intValue << driverProperty.BitOffset;
					}

					if (driverProperty.UseMask)
					{
						binProperty.HighByte += intValue;
						binProperty.LowByte = 0xFF;
					}
					else
					{
						if (driverProperty.HighByte)
							binProperty.LowByte += intValue;
						else
							binProperty.HighByte += intValue;
					}
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