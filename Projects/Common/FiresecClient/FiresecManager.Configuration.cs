using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static List<Driver> Drivers { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static DeviceConfigurationStates DeviceStates { get; set; }
		public static LibraryConfiguration LibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }

		public static void GetConfiguration(bool updateFiles = true)
		{
			if (updateFiles)
				FileHelper.Synchronize();

			SystemConfiguration = FiresecService.GetSystemConfiguration();
			LibraryConfiguration = FiresecService.GetLibraryConfiguration();
			PlansConfiguration = FiresecService.GetPlansConfiguration();
			SecurityConfiguration = FiresecService.GetSecurityConfiguration();
			Drivers = FiresecService.GetDrivers();
			DeviceConfiguration = FiresecService.GetDeviceConfiguration();

			UpdateDrivers();
			UpdateConfiguration();
		}

		public static void UpdateDrivers()
		{
			if (Drivers != null)
			{
				Drivers.ForEach(x => x.ImageSource = FileHelper.GetIconFilePath(x.ImageSource) + ".ico");
			}
		}

		public static void UpdateConfiguration()
		{
			PlansConfiguration.Update();
			DeviceConfiguration.Update();
			ReorderConfiguration();

			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver.IsIndicatorDevice || device.IndicatorLogic != null)
					device.IndicatorLogic.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);

				if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
				{
					foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.DeviceUID != Guid.Empty))
					{
						clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
					}
				}
			}

			InvalidateConfiguration();
			UpdatePlansConfiguration();
		}

		public static void UpdatePlansConfiguration()
		{
			FiresecManager.DeviceConfiguration.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				for (int i = plan.ElementDevices.Count(); i > 0; i--)
				{
					var elementDevice = plan.ElementDevices[i - 1];

					var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
					if (device != null)
						device.PlanElementUIDs.Add(elementDevice.UID);
					else
						plan.ElementDevices.RemoveAt(i - 1);
				}
			}
		}

		public static void InvalidateConfiguration()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.Indicator)
				{
					if (DeviceConfiguration.Devices.Any(x => x.UID == device.IndicatorLogic.DeviceUID) == false)
					{
						device.IndicatorLogic.DeviceUID = Guid.Empty;
						device.IndicatorLogic.Device = null;
					}

					var zones = new List<ulong>();
					foreach (var zoneNo in device.IndicatorLogic.Zones)
					{
						if (DeviceConfiguration.Zones.Any(x => x.No == zoneNo))
							zones.Add(zoneNo);
					}
					device.IndicatorLogic.Zones = zones;
				}
				if (device.Driver.DriverType == DriverType.PDUDirection)
				{
					foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
					{
						if (DeviceConfiguration.Devices.Any(x => x.UID == pduGroupDevice.DeviceUID) == false)
							pduGroupDevice.DeviceUID = Guid.Empty;
					}
				}
				if (device.Driver.IsZoneDevice)
				{
					if (DeviceConfiguration.Zones.Any(x => x.No == device.ZoneNo) == false)
						device.ZoneNo = null;
				}
				if (device.Driver.IsZoneLogicDevice)
				{
					if (device.ZoneLogic != null)
					{
						var clauses = new List<Clause>();
						foreach (var clause in device.ZoneLogic.Clauses)
						{
							if (DeviceConfiguration.Devices.Any(x => x.UID == clause.DeviceUID) == false)
							{
								clause.DeviceUID = Guid.Empty;
								clause.Device = null;
							}

							var zones = new List<ulong>();
							foreach (var zoneNo in clause.Zones)
							{
								if (DeviceConfiguration.Zones.Any(x => x.No == zoneNo))
									zones.Add(zoneNo);
							}
							clause.Zones = zones;

							if ((clause.Device != null) || (clause.Zones.Count > 0))
								clauses.Add(clause);
						}
						device.ZoneLogic.Clauses = clauses;
					}
				}

				if (device.Driver.DriverType != DriverType.Indicator)
					device.IndicatorLogic = null;
				if (device.Driver.DriverType != DriverType.PDUDirection)
					device.PDUGroupLogic = null;
				if (!device.Driver.IsZoneDevice)
					device.ZoneNo = null;
				if (!device.Driver.IsZoneLogicDevice)
					device.ZoneLogic = null;
			}

			UpdateZoneDevices();
		}

		public static List<Zone> GetChannelZones(Device device)
		{
			UpdateZoneDevices();

			var zones = new List<Zone>();

			var channelDevice = device.ParentChannel;

			foreach (var zone in from zone in FiresecManager.DeviceConfiguration.Zones orderby zone.No select zone)
			{
				if (channelDevice != null)
				{
					if (zone.DevicesInZone.All(x => ((x.ParentChannel != null) && (x.ParentChannel.UID == channelDevice.UID))))
						zones.Add(zone);
				}
				else
				{
					if (zone.DevicesInZone.All(x => (x.Parent.UID == device.UID)))
						zones.Add(zone);
				}
			}

			return zones;
		}

		public static void UpdateZoneDevices()
		{
			foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
			{
				zone.DevicesInZone = new List<Device>();
				zone.DeviceInZoneLogic = new List<Device>();
			}

			foreach (var device in FiresecManager.DeviceConfiguration.Devices)
			{
				if (device.Driver == null)
				{
					System.Windows.MessageBox.Show("У устройства отсутствует драйвер");
					continue;
				}

				if ((device.Driver.IsZoneDevice) && (device.ZoneNo != null))
				{
					var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
					if (zone != null)
					{
						zone.DevicesInZone.Add(device);
					}
				}
				if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
				{
					foreach (var clause in device.ZoneLogic.Clauses)
					{
						foreach (var clauseZone in clause.Zones)
						{
							var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == clauseZone);
							if (zone != null)
							{
								zone.DeviceInZoneLogic.Add(device);
							}
						}
					}
				}
			}
		}

		public static bool HasExternalDevices(Device device)
		{
			if (device.ZoneLogic != null)
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					foreach (var zoneNo in clause.Zones)
					{
						var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
						if (zone != null)
						{
							foreach (var deviceInZone in zone.DevicesInZone)
							{
								if (device.ParentPanel.UID != deviceInZone.ParentPanel.UID)
									return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static void ReorderConfiguration()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				if (device.Children.Count > 0)
				{
					device.Children = new List<Device>(device.Children.OrderBy(x => { return x.IntAddress; }));
				}
			}
		}
	}
}