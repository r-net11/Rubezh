using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Processor;
using System.Diagnostics;

namespace FiresecService.Service
{
	public partial class FiresecService
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
				if (property.IsInternalDeviceParameter)
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
						Error = result.ErrorString
					};
				}
				Firesec.DeviceCustomFunctions.requests requests = SerializerHelper.Deserialize<Firesec.DeviceCustomFunctions.requests>(result.Result);
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
									if (driverProperty.No == 0x85)
									{
										;
									}
									var offsetParamValue = paramValue;

									if (driverProperty.IsHeighByte)
										offsetParamValue = lowByteValue;
									if (driverProperty.IsLowByte)
										offsetParamValue = heightByteValue;

									if (driverProperty.MinOffset > 0)
									{
										byte byteOffsetParamValue = (byte)offsetParamValue;
										byteOffsetParamValue = (byte)(byteOffsetParamValue >> driverProperty.MinOffset);
										byteOffsetParamValue = (byte)(byteOffsetParamValue << driverProperty.MinOffset);
										offsetParamValue = byteOffsetParamValue;
									}
									if (driverProperty.MaxOffset > 0)
									{
										byte byteOffsetParamValue = (byte)offsetParamValue;
										byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - driverProperty.MaxOffset);
										byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - driverProperty.MaxOffset);
										offsetParamValue = byteOffsetParamValue;
									}
									if (driverProperty.Offset > 0)
									{
										offsetParamValue = offsetParamValue >> driverProperty.Offset;
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
				if (waitCount > 60)
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
				if (driverProperty != null && driverProperty.IsInternalDeviceParameter)
				{
					var binProperty = binProperties.FirstOrDefault(x=>x.No == driverProperty.No);
					if (binProperty == null)
					{
						binProperty = new BinProperty()
						{
							No = driverProperty.No,
							Value = 0
						};
						binProperties.Add(binProperty);
					}

					int newValue = 0;
					if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.EnumType)
					{
						var driverPropertyParameterValue = driverProperty.Parameters.FirstOrDefault(x => x.Name == property.Value);
						if (driverPropertyParameterValue != null)
						{
							newValue = int.Parse(driverPropertyParameterValue.Value);
						}
					}
					else
					{
						newValue = int.Parse(property.Value);

						if (device.Driver.DriverType == DriverType.MRO && driverProperty.Name == "Время отложенного пуска")
						{
							newValue = (int)Math.Truncate((double)newValue / 5);
						}
					}

					if (driverProperty.Offset > 0)
					{
						newValue = newValue << driverProperty.Offset;
					}

					//if (driverProperty.IsHeighByte)
					//{
					//    newValue *= 256;
					//}


					binProperty.Value += newValue;
				}
			}

			foreach (var binProperty in binProperties)
			{
				if (binProperty.No == 0x85)
				{
					;
				}
				Trace.WriteLine(binProperty.ToString());
				int requestId = 0;
				binProperty.Value *= 256;
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
			public int Value;
			public override string ToString()
			{
				return No.ToString() + "=" + Value.ToString();
			}
		}
	}
}