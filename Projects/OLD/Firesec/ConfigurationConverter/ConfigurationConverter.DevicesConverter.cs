using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.Models.CoreConfiguration;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		void ConvertDevices(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			deviceConfiguration.Devices = new List<Device>();

			if (coreConfig == null || coreConfig.dev == null || coreConfig.dev.Count() == 0 || coreConfig.drv == null)
			{
				Logger.Error("ConfigurationConverter.ConvertDevices coreConfig.dev = null");
				LoadingErrorManager.Add("Пустая коллекция устройств или драйверов при конвертации конфигурации");
				return;
			}

			var rootInnerDevice = coreConfig.dev[0];
			var rootDevice = SetInnerDevice(rootInnerDevice, null, deviceConfiguration, coreConfig);
			deviceConfiguration.Devices.Add(rootDevice);
			AddDevice(rootInnerDevice, rootDevice, deviceConfiguration, coreConfig);
			deviceConfiguration.RootDevice = rootDevice;
		}

		void AddDevice(devType parentInnerDevice, Device parentDevice, DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			if (parentInnerDevice.dev == null)
				return;

			parentDevice.Children = new List<Device>();
			foreach (var innerDevice in parentInnerDevice.dev)
			{
				var device = SetInnerDevice(innerDevice, parentDevice, deviceConfiguration, coreConfig);
				if (device != null)
				{
					parentDevice.Children.Add(device);
					deviceConfiguration.Devices.Add(device);
					AddDevice(innerDevice, device, deviceConfiguration, coreConfig);
				}
			}
		}

		Device SetInnerDevice(devType innerDevice, Device parentDevice, DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var device = new Device()
			{
				Parent = parentDevice
			};
			var drvType = coreConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv);
			if(drvType == null)
			{
				Logger.Error("ConfigurationConverter.SetInnerDevice drvType = null " + innerDevice.drv.ToString());
				LoadingErrorManager.Add("Ошибка сопоставления при конвертации конфигурации");
				return null;
			}
			var driverUID = new Guid(drvType.id);
			device.DriverUID = driverUID;
			device.Driver = ConfigurationCash.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == driverUID);
			if (device.Driver == null)
			{
				Logger.Error("ConvertDevices.SetInnerDevice driver = null " + driverUID.ToString());
				LoadingErrorManager.Add("Неизвестный драйвер устройства " + driverUID.ToString());
				return null;
			}

			device.IntAddress = int.Parse(innerDevice.addr);
			if ((device.Parent != null) && (device.Parent.Driver.IsChildAddressReservedRange))
				device.IntAddress += device.Parent.IntAddress;

			if ((innerDevice.disabled != null) && (innerDevice.disabled == "1"))
				device.IsMonitoringDisabled = true;

			if (innerDevice.param != null)
			{
				var DatabaseIdParam = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices");
				if (DatabaseIdParam != null)
					device.DatabaseId = DatabaseIdParam.value;

				var UIDParam = innerDevice.param.FirstOrDefault(x => x.name == "INT$DEV_GUID");
				if (UIDParam != null)
					device.UID = GuidHelper.ToGuid(UIDParam.value);
				else
					device.UID = Guid.NewGuid();
			}

			if (innerDevice.dev_param != null)
			{
				var AltInterfaceParam = innerDevice.dev_param.FirstOrDefault(x => x.name == "SYS$Alt_Interface");
				if (AltInterfaceParam != null)
					device.IsAltInterface = true;
				else
					device.IsAltInterface = false;
			}

			device.Properties = new List<Property>();
			if (innerDevice.prop != null)
			{
				foreach (var innerProperty in innerDevice.prop)
				{
					if (innerProperty.name == "IsAlarmDevice")
					{
						device.IsRmAlarmDevice = true;
						continue;
					}
					if (innerProperty.name == "NotUsed")
					{
						device.IsNotUsed = true;
						continue;
					}
					device.Properties.Add(new Property()
					{
						Name = innerProperty.name,
						Value = innerProperty.value
					});
				}
			}

			var description = innerDevice.name;
			if (description != null)
				description = description.Replace('¹', '№');
			device.Description = description;
			SetZone(device, innerDevice, deviceConfiguration, coreConfig);

			device.ShapeIds = new List<string>();
			if (innerDevice.shape != null)
			{
				foreach (var shape in innerDevice.shape)
				{
					device.ShapeIds.Add(shape.id);
				}
			}

			device.PlaceInTree = device.GetPlaceInTree();
			return device;
		}

		void SetZone(Device device, devType innerDevice, DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			if (innerDevice.inZ != null && innerDevice.inZ.Count() > 0)
			{
				string zoneIdx = innerDevice.inZ[0].idz;
				string zoneNo = coreConfig.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
				int intZoneNo = int.Parse(zoneNo);
				var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.No == intZoneNo);
				if (zone != null)
				{
					device.ZoneUID = zone.UID;
				}
			}
			if (innerDevice.prop != null)
			{
				var zoneLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
				if (zoneLogicProperty != null)
				{
					string zoneLogicstring = zoneLogicProperty.value;
					if (string.IsNullOrEmpty(zoneLogicstring) == false)
						device.ZoneLogic = ZoneLogicConverter.Convert(deviceConfiguration, SerializerHelper.GetZoneLogic(zoneLogicstring));
				}

				var indicatorLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
				if (indicatorLogicProperty != null)
				{
					string indicatorLogicString = indicatorLogicProperty.value;
					if (string.IsNullOrEmpty(indicatorLogicString) == false)
					{
						var indicatorLogic = SerializerHelper.GetIndicatorLogic(indicatorLogicString);
						if (indicatorLogic != null)
						{
							device.IndicatorLogic = IndicatorLogicConverter.Convert(deviceConfiguration, indicatorLogic);
						}
					}
				}

				var pDUGroupLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
				if (pDUGroupLogicProperty != null)
				{
					string pDUGroupLogicPropertyString = pDUGroupLogicProperty.value;
					if (string.IsNullOrEmpty(pDUGroupLogicPropertyString) == false)
						device.PDUGroupLogic = PDUGroupLogicConverter.Convert(SerializerHelper.GetGroupProperties(pDUGroupLogicPropertyString));
				}
			}
		}

		void ConvertDevicesBack(DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			ConvertDriversBack(coreConfig);
			var rootDevice = deviceConfiguration.RootDevice;
			var rootInnerDevice = DeviceToInnerDevice(rootDevice, deviceConfiguration, coreConfig);
			AddInnerDevice(rootDevice, rootInnerDevice, deviceConfiguration, coreConfig);

			coreConfig.dev = new devType[1];
			coreConfig.dev[0] = rootInnerDevice;
		}

		void ConvertDriversBack(Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var drivers = new List<drvType>();
			foreach (var driver in ConfigurationCash.DriversConfiguration.Drivers)
			{
				var innerDriver = new drvType()
				{
					idx = ConfigurationCash.DriversConfiguration.Drivers.IndexOf(driver).ToString(),
					id = driver.StringUID,
					name = driver.Name
				};
				drivers.Add(innerDriver);
			}
			coreConfig.drv = drivers.ToArray();
		}

		void AddInnerDevice(Device parentDevice, devType parentInnerDevice, DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var childInnerDevices = new List<devType>();
			foreach (var device in parentDevice.Children)
			{
				var childInnerDevice = DeviceToInnerDevice(device, deviceConfiguration, coreConfig);
				childInnerDevices.Add(childInnerDevice);
				AddInnerDevice(device, childInnerDevice, deviceConfiguration, coreConfig);
			}
			parentInnerDevice.dev = childInnerDevices.ToArray();
		}

		devType DeviceToInnerDevice(Device device, DeviceConfiguration deviceConfiguration, Firesec.Models.CoreConfiguration.config coreConfig)
		{
			var innerDevice = new devType();
			innerDevice.name = device.Description;
			if (innerDevice.name != null)
				innerDevice.name = innerDevice.name.Replace('№', 'N');

			innerDevice.drv = coreConfig.drv.FirstOrDefault(x => x.id.ToUpper() == device.Driver.StringUID).idx;

			var intAddress = device.IntAddress;
			if ((device.Parent != null) && (device.Parent.Driver.IsChildAddressReservedRange))
				intAddress -= device.Parent.IntAddress;

			if (device.Driver.HasAddress)
				innerDevice.addr = intAddress.ToString();
			else
				innerDevice.addr = "0";

			if (device.IsMonitoringDisabled == true)
				innerDevice.disabled = "1";
			else
				innerDevice.disabled = null;

			if (device.ZoneUID != Guid.Empty)
			{
				var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
				if (zone != null)
				{
					var zones = new List<inZType>();
					zones.Add(new inZType() { idz = zone.No.ToString() });
					innerDevice.inZ = zones.ToArray();
				}
			}

			innerDevice.prop = AddProperties(device).ToArray();
			innerDevice.param = AddParameters(device).ToArray();
			innerDevice.dev_param = AddDevParameters(device).ToArray();
			innerDevice.shape = AddShapes(device).ToArray();

			return innerDevice;
		}

		List<paramType> AddParameters(Device device)
		{
			var parameters = new List<paramType>();
			{
				if (device.UID != Guid.Empty)
				{
					parameters.Add(new paramType()
					{
						name = "INT$DEV_GUID",
						type = "String",
						value = GuidHelper.ToString(device.UID)
					});
				}
			}

			if (device.DatabaseId != null)
			{
				parameters.Add(new paramType()
				{
					name = "DB$IDDevices",
					type = "Int",
					value = device.DatabaseId
				});
			}

			return parameters;
		}

		List<dev_paramType> AddDevParameters(Device device)
		{
			var devParameters = new List<dev_paramType>();
			if (device.IsAltInterface)
			{
				devParameters.Add(new dev_paramType()
				{
					name = "SYS$Alt_Interface",
					type = "String",
					value = "USB"
				});
			}
			return devParameters;
		}

		List<propType> AddProperties(Device device)
		{
			var propertyList = new List<propType>();
			if (device.Driver.DriverType != DriverType.Computer && device.Properties.IsNotNullOrEmpty())
			{
				foreach (var deviceProperty in device.Properties)
				{
					var property = device.Driver.Properties.FirstOrDefault(x => x.Name == deviceProperty.Name);
					if (property != null && property.IsAUParameter)
					{
						continue;
					}

					if (string.IsNullOrEmpty(deviceProperty.Name) == false &&
						string.IsNullOrEmpty(deviceProperty.Value) == false)
					{
						propertyList.Add(new propType()
						{
							name = deviceProperty.Name,
							value = deviceProperty.Value
						});
					}
				}
			}

			if (device.IsRmAlarmDevice)
			{
				var isRmAlarmDeviceProperty = propertyList.FirstOrDefault(x => x.name == "IsAlarmDevice");
				if (isRmAlarmDeviceProperty == null)
				{
					propertyList.Add(new propType() { name = "IsAlarmDevice", value = "1" });
				}
			}

			if (device.IsNotUsed)
			{
				var isNotUsedProperty = propertyList.FirstOrDefault(x => x.name == "NotUsed");
				if (isNotUsedProperty == null)
				{
					propertyList.Add(new propType() { name = "NotUsed", value = "1" });
				}
			}

			if (device.ZoneLogic != null)
			{
				if (device.ZoneLogic.Clauses.Count > 0)
				{
					var zoneLogicProperty = propertyList.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
					if (zoneLogicProperty == null)
					{
						zoneLogicProperty = new propType();
						propertyList.Add(zoneLogicProperty);
					}

					zoneLogicProperty.name = "ExtendedZoneLogic";
					var zoneLogic = ZoneLogicConverter.ConvertBack(device.ZoneLogic);
					zoneLogicProperty.value = SerializerHelper.SetZoneLogic(zoneLogic);
				}
			}

			if ((device.Driver.DriverType == DriverType.Indicator) && (device.IndicatorLogic != null))
			{
				var indicatorLogicProperty = propertyList.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
				if (indicatorLogicProperty == null)
				{
					indicatorLogicProperty = new propType();
					propertyList.Add(indicatorLogicProperty);
				}

				indicatorLogicProperty.name = "C4D7C1BE-02A3-4849-9717-7A3C01C23A24";
				var indicatorLogic = IndicatorLogicConverter.ConvertBack(device.IndicatorLogic);
				indicatorLogicProperty.value = SerializerHelper.SetIndicatorLogic(indicatorLogic);
			}

			if ((device.Driver.DriverType == DriverType.PDUDirection) || (device.Driver.DriverType == DriverType.PDU_PTDirection))
			{
				var pduGroupLogicProperty = propertyList.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
				if (pduGroupLogicProperty == null)
				{
					pduGroupLogicProperty = new propType();
					propertyList.Add(pduGroupLogicProperty);
				}

				pduGroupLogicProperty.name = "E98669E4-F602-4E15-8A64-DF9B6203AFC5";
				var pDUGroupLogic = PDUGroupLogicConverter.ConvertBack(device.PDUGroupLogic);
				pduGroupLogicProperty.value = SerializerHelper.SeGroupProperty(pDUGroupLogic);
			}

			return propertyList;
		}

		List<shapeType> AddShapes(Device device)
		{
			var shapeTypes = new List<shapeType>();
			if (device.ShapeIds != null)
			foreach (var shapeId in device.ShapeIds)
			{
				var shape = new shapeType()
				{
					id = shapeId
				};
				shapeTypes.Add(shape);
			}
			return shapeTypes;
		}
	}
}