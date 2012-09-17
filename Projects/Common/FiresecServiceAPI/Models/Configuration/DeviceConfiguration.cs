using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceConfiguration : VersionedConfiguration
	{
		public DeviceConfiguration()
		{
			Devices = new List<Device>();
			Zones = new List<Zone>();
			Directions = new List<Direction>();
			GuardUsers = new List<GuardUser>();
		}

		public List<Device> Devices { get; set; }

		[DataMember]
		public Device RootDevice { get; set; }

		[DataMember]
		public List<Zone> Zones { get; set; }

		[DataMember]
		public List<Direction> Directions { get; set; }

		[DataMember]
		public List<GuardUser> GuardUsers { get; set; }

		public void Update()
		{
			Devices = new List<Device>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(Device parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		public void UpdateCrossReferences()
		{
			foreach (var zone in Zones)
			{
				zone.DevicesInZone = new List<Device>();
				zone.DeviceInZoneLogic = new List<Device>();
			}
			foreach (var device in Devices)
			{
				device.Zone = null;
				device.ZonesInLogic = new List<Zone>();
			}
			foreach (var device in Devices)
			{
				device.Zone = null;
				if ((device.Driver.IsZoneDevice) && (device.ZoneNo != null))
				{
					var zone = Zones.FirstOrDefault(x=>x.No == device.ZoneNo);
					device.Zone = zone;
					zone.DevicesInZone.Add(device);
				}
				if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
				{
					foreach (var clause in device.ZoneLogic.Clauses)
					{
						foreach (var clauseZone in clause.ZoneNos)
						{
							var zone = Zones.FirstOrDefault(x => x.No == clauseZone);
							if (zone != null)
							{
								zone.DeviceInZoneLogic.Add(device);
								device.ZonesInLogic.Add(zone);
							}
						}
					}
				}
			}
		}

		public void InvalidateConfiguration()
		{
			foreach (var device in Devices)
			{
				InvalidateOneDevice(device);
			}
			UpdateCrossReferences();
		}

		public void InvalidateOneDevice(Device device)
		{
			if (device.Driver.DriverType == DriverType.Indicator)
			{
				InvalidateIndicator(device);
			}
			if (device.Driver.DriverType == DriverType.PDUDirection)
			{
				InvalidatePDUDirection(device);
			}
			if (device.Driver.IsZoneDevice)
			{
				if (Zones.Any(x => x.No == device.ZoneNo) == false)
					device.ZoneNo = null;
			}
			if (device.Driver.IsZoneLogicDevice)
			{
				if (device.ZoneLogic != null)
				{
					var clauses = new List<Clause>();
					foreach (var clause in device.ZoneLogic.Clauses)
					{
						if (Devices.Any(x => x.UID == clause.DeviceUID) == false)
						{
							clause.DeviceUID = Guid.Empty;
							clause.Device = null;
						}

						var zoneNos = new List<int>();
						var zones = new List<Zone>();
						if (clause.ZoneNos == null)
							clause.ZoneNos = new List<int>();
						foreach (var zoneNo in clause.ZoneNos)
						{
							var zone = Zones.FirstOrDefault(x => x.No == zoneNo);
							if (zone != null)
							{
								zoneNos.Add(zoneNo);
								zones.Add(zone);
							}
						}
						clause.ZoneNos = zoneNos;
						clause.Zones = zones;

						if ((clause.Device != null) || (clause.ZoneNos.Count > 0) || clause.State == ZoneLogicState.Failure)
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

		public void InvalidateIndicator(Device device)
		{
			if (device.Driver.DriverType == DriverType.Indicator)
			{
				device.IndicatorLogic.Device = Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);
				if (device.IndicatorLogic.Device == null)
				{
					device.IndicatorLogic.DeviceUID = Guid.Empty;
				}

				var zoneNos = new List<int>();
				var zones = new List<Zone>();
				if (device.IndicatorLogic.ZoneNos == null)
				{
					device.IndicatorLogic.ZoneNos = new List<int>();
				}
				foreach (var zoneNo in device.IndicatorLogic.ZoneNos)
				{
					var zone = Zones.FirstOrDefault(x => x.No == zoneNo);
					if (zone != null)
					{
						zoneNos.Add(zoneNo);
						zones.Add(zone);
					}
				}
				device.IndicatorLogic.ZoneNos = zoneNos;
				device.IndicatorLogic.Zones = zones;
			}
		}

		public void InvalidatePDUDirection(Device device)
		{
			foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
			{
				if (Devices.Any(x => x.UID == pduGroupDevice.DeviceUID) == false)
					pduGroupDevice.DeviceUID = Guid.Empty;
			}
		}

		public void UpdateGuardConfiguration()
		{
			foreach (var guardUser in GuardUsers)
			{
				var device = Devices.FirstOrDefault(x => x.UID == guardUser.DeviceUID);
				if (device == null)
				{
					guardUser.DeviceUID = Guid.Empty;
				}
				else
				{
					var userZones = new List<int>();
					foreach (var zoneNo in guardUser.Zones)
					{
						var zone = Zones.FirstOrDefault(x => x.No == zoneNo);
						if (zone != null)
							userZones.Add(zoneNo);
					}
					guardUser.Zones = userZones;
				}
			}
		}

		public DeviceConfiguration CopyOneBranch(Device device, bool isUsb)
		{
			var deviceConfiguration = new DeviceConfiguration();

			Device currentDevice = device;
			Device copyChildDevice = null;

			while (true)
			{
				var copyDevice = new Device()
				{
					UID = currentDevice.UID,
					DriverUID = currentDevice.DriverUID,
					IntAddress = currentDevice.IntAddress,
					Description = currentDevice.Description,
					ZoneNo = currentDevice.ZoneNo,
					Properties = new List<Property>(currentDevice.Properties)
				};
				if ((currentDevice.UID == device.UID))
				{
					copyDevice.IsAltInterface = isUsb;
				}

				if (copyChildDevice != null)
					copyDevice.Children.Add(copyChildDevice);

				if (currentDevice.Parent == null)
				{
					currentDevice = copyDevice;
					break;
				}
				copyChildDevice = copyDevice;
				currentDevice = currentDevice.Parent;
			}

			deviceConfiguration.RootDevice = currentDevice;
			return deviceConfiguration;
		}

		public void UpdateIdPath()
		{
			if (RootDevice != null)
			{
				RootDevice.PathId = RootDevice.Driver.UID.ToString() + ":" + RootDevice.AddressFullPath;
				UpdateChildIdPath(RootDevice);
			}
		}

		void UpdateChildIdPath(Device parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.PathId = device.Parent.PathId + @"/" + device.Driver.UID.ToString() + ":" + device.AddressFullPath; ;
				UpdateChildIdPath(device);
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				if (device.Children.Count > 0)
				{
					device.Children = new List<Device>(device.Children.OrderBy(x => { return x.IntAddress; }));
				}
			}
		}
	}
}