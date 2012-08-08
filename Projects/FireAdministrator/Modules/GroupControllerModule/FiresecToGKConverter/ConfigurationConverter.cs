using System;
using System.Linq;
using Common.GK;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter
{
	public class ConfigurationConverter
	{
		public void Convert()
		{
			XManager.DeviceConfiguration = new XDeviceConfiguration();
			ConvertDevices();
			ConvertZones();
			ConvertLogic();
		}

		void ConvertDevices()
		{
			byte kauAddress = 1;

			var systemDevice = new XDevice()
			{
				UID = Guid.NewGuid(),
				DriverUID = GKDriversHelper.System_UID,
				Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
			};
			XManager.DeviceConfiguration.Devices.Add(systemDevice);
			XManager.DeviceConfiguration.RootDevice = systemDevice;

			var gkDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.GK);
			var gkDevice = XManager.AddChild(systemDevice, gkDriver, 0, 1);
			gkDevice.UID = Guid.NewGuid();
			XManager.DeviceConfiguration.Devices.Add(gkDevice);

			foreach (var device in FiresecManager.Devices)
			{
				bool isKAU = false;
				var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);

				if (driver == null)
				{
					switch (device.Driver.DriverType)
					{
						case FiresecAPI.Models.DriverType.BUNS:
						case FiresecAPI.Models.DriverType.BUNS_2:
						case FiresecAPI.Models.DriverType.Rubezh_10AM:
						case FiresecAPI.Models.DriverType.Rubezh_2AM:
						case FiresecAPI.Models.DriverType.Rubezh_2OP:
						case FiresecAPI.Models.DriverType.Rubezh_4A:
						case FiresecAPI.Models.DriverType.USB_BUNS:
						case FiresecAPI.Models.DriverType.USB_BUNS_2:
						case FiresecAPI.Models.DriverType.USB_Rubezh_2AM:
						case FiresecAPI.Models.DriverType.USB_Rubezh_2OP:
						case FiresecAPI.Models.DriverType.USB_Rubezh_4A:
							isKAU = true;
							driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
							break;

						default:
							continue;
					}
				}

				if (isKAU)
				{
					var kauDevice = XManager.AddChild(gkDevice, driver, 0, kauAddress++);
					kauDevice.UID = device.UID;
					XManager.DeviceConfiguration.Devices.Add(kauDevice);
				}
				else
				{
					var xDevice = new XDevice()
					{
						UID = device.UID,
						DriverUID = driver.UID,
						Driver = driver,
						ShleifNo = (byte)(device.IntAddress >> 8),
						IntAddress = (byte)(device.IntAddress & 0xff),
						Description = device.Description
					};
					XManager.DeviceConfiguration.Devices.Add(xDevice);
				}
			}

			foreach (var device in FiresecManager.Devices)
			{
				var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
				if (xDevice != null)
				{
					foreach (var child in device.Children)
					{
						var xChildDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == child.UID);
						if (xChildDevice != null)
						{
							xDevice.Children.Add(xChildDevice);
							xChildDevice.Parent = xDevice;
						}
					}
				}
			}
		}

		void ConvertZones()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				var xZone = new XZone()
				{
					No = (short)zone.No,
					Name = zone.Name,
					Description = zone.Description,
					DetectorCount = (short)zone.DetectorCount,
				};
				XManager.DeviceConfiguration.Zones.Add(xZone);
			}

			foreach (var device in FiresecManager.Devices)
			{
				var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
				if ((device.Driver.IsZoneDevice) && (device.ZoneNo.HasValue))
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.ZoneNo.Value);
					var xZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == (short)zone.No);
					if (zone != null)
					{
						xDevice.Zones.Add(xZone.No);
						xZone.DeviceUIDs.Add(device.UID);
					}
				}
			}
		}

		void ConvertLogic()
		{
			foreach (var xDevice in XManager.DeviceConfiguration.Devices)
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == xDevice.UID);
				if (device != null)
				{
					if ((device.Driver.IsZoneDevice) && (device.ZoneLogic != null))
					{
						var xDeviceLogic = new XDeviceLogic();
						var stateLogic = new StateLogic()
						{
							StateType = XStateType.TurnOn
						};
						xDeviceLogic.StateLogics.Add(stateLogic);

						foreach (var clause in device.ZoneLogic.Clauses)
						{
							var xClause = new XClause();
							switch (clause.State)
							{
								case FiresecAPI.Models.ZoneLogicState.Fire:
									xClause.StateType = XStateType.Fire1;
									break;

								case FiresecAPI.Models.ZoneLogicState.Failure:
									xClause.StateType = XStateType.Failure;
									break;

								default:
									continue;
							}
							if ((clause.Zones == null) || (clause.Zones.Count == 0))
								continue;

							foreach (var zoneNo in clause.Zones)
							{
								xClause.Zones.Add((short)zoneNo);
							}

							stateLogic.Clauses.Add(xClause);
						}

						if (stateLogic.Clauses.Count > 0)
							xDevice.DeviceLogic = xDeviceLogic;
					}
				}
			}
		}
	}
}