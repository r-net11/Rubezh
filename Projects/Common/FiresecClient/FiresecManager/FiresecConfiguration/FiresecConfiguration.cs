using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public List<Driver> Drivers { get; set; }
		public DeviceConfiguration DeviceConfiguration { get; set; }

		public void UpdateDrivers()
		{
			if (Drivers != null)
			{
				Drivers.ForEach(x => x.ImageSource = IconHelper.GetFSIcon(x));
			}
		}

		public void UpdateConfiguration()
		{
			DeviceConfiguration.Update();
			ReorderConfiguration();

			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
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

			UpdateChildMPTZones();
			InvalidateConfiguration();
			UpateGuardZoneSecPanelUID();
		}

		public void ReorderConfiguration()
		{
			DeviceConfiguration.Reorder();
		}

		void UpdateChildMPTZones()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.MPT)
				{
					foreach (var child in device.Children)
					{
						child.ZoneNo = device.ZoneNo;
					}
				}
			}
		}

		public void InvalidateConfiguration()
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

					var zones = new List<int>();
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

							var zones = new List<int>();
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

		public void UpdateZoneDevices()
		{
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.DevicesInZone = new List<Device>();
				zone.DeviceInZoneLogic = new List<Device>();
			}

			foreach (var device in DeviceConfiguration.Devices)
			{
				if (device.Driver == null)
				{
					System.Windows.MessageBox.Show("У устройства отсутствует драйвер");
					continue;
				}

				if ((device.Driver.IsZoneDevice) && (device.ZoneNo != null))
				{
					var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
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
							var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == clauseZone);
							if (zone != null)
							{
								zone.DeviceInZoneLogic.Add(device);
							}
						}
					}
				}
			}
		}

		public List<Zone> GetChannelZones(Device device)
		{
			UpdateZoneDevices();
			var zones = new List<Zone>();
			var channelDevice = device.ParentChannel;

			foreach (var zone in from zone in DeviceConfiguration.Zones orderby zone.No select zone)
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

		public List<Zone> GetPanelZones(Device device)
		{
			UpdateZoneDevices();
			var zones = new List<Zone>();
			var channelDevice = device.ParentPanel;

			foreach (var zone in from zone in DeviceConfiguration.Zones orderby zone.No select zone)
			{
				if (channelDevice != null)
				{
					if (zone.DevicesInZone.All(x => ((x.ParentPanel != null) && (x.ParentPanel.UID == channelDevice.UID))))
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

		public bool HasExternalDevices(Device device)
		{
			if (device.ZoneLogic != null)
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					foreach (var zoneNo in clause.Zones)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
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

		List<Device> allChildren;
		public List<Device> GetAllChildrenForDevice(Device device)
		{
			allChildren = new List<Device>();
			AddChild(device);
			return allChildren;
		}
		void AddChild(Device device)
		{
			foreach (var child in device.Children)
			{
				allChildren.Add(child);
				if (child.Driver.DriverType == DriverType.MPT)
				{
					AddChild(child);
				}
			}
		}

		public bool IsChildMPT(Device device)
		{
			if (device.Parent == null)
				return false;
			return ((device.Driver.DriverType == DriverType.MPT) && (device.Parent.Driver.DriverType == DriverType.MPT));
		}

		public void UpateGuardZoneSecPanelUID()
		{
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.SecPanelUID = Guid.Empty;
				if (zone.ZoneType == ZoneType.Guard)
				{
					foreach (var device in zone.DevicesInZone)
					{
						if (device.Driver.DriverType == DriverType.AM1_O)
						{
							zone.SecPanelUID = device.Parent.UID;
							break;
						}
					}
				}
			}
		}

		public int GetZoneLocalSecNo(Zone zone)
		{
			if (zone.SecPanelUID != Guid.Empty)
			{
				var guardZones = (from guardZone in DeviceConfiguration.Zones
								  orderby guardZone.No
								  where guardZone.SecPanelUID == zone.SecPanelUID
								  select guardZone).ToList();
				return guardZones.IndexOf(zone) + 1;
			}
			return -1;
		}

		List<Zone> GetPanelLocalZones(Device device)
		{
			var zones = new List<Zone>();
			foreach (var child in GetPanelChildren(device))
			{
				if (child.Driver.IsZoneDevice)
				{
					if (child.ZoneNo.HasValue)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == child.ZoneNo);
						if (zone != null)
						{
							zones.Add(zone);
						}
					}
				}
			}
			return zones;
		}

		List<Device> panelChildren;
		List<Device> GetPanelChildren(Device device)
		{
			panelChildren = new List<Device>();
			AddPanelChildren(device);
			return panelChildren;
		}
		void AddPanelChildren(Device device)
		{
			foreach (var child in device.Children)
			{
				panelChildren.Add(child);
			}
		}
	}
}