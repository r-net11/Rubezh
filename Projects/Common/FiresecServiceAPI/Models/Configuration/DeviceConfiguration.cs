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
					var userZones = new List<ulong>();
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

		public DeviceConfiguration CopyOneBranch(Guid uid, bool isUsb)
		{
			var deviceConfiguration = new DeviceConfiguration();

			var device = Devices.FirstOrDefault(x => x.UID == uid);
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
				if ((currentDevice.UID == uid))
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
	}
}