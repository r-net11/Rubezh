using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public partial class DeviceConfiguration : VersionedConfiguration
	{
		public DeviceConfiguration()
		{
			Devices = new List<Device>();
			Zones = new List<Zone>();
			Directions = new List<Direction>();
			GuardUsers = new List<GuardUser>();
			ParameterTemplates = new List<ParameterTemplate>();
		}

		[XmlIgnore]
		public List<Device> Devices { get; set; }

		[DataMember]
		public Device RootDevice { get; set; }

		[DataMember]
		public List<Zone> Zones { get; set; }

		[DataMember]
		public List<Direction> Directions { get; set; }

		[DataMember]
		public List<GuardUser> GuardUsers { get; set; }

		[DataMember]
		public List<ParameterTemplate> ParameterTemplates { get; set; }

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
					var userZones = new List<Guid>();
					foreach (var zoneUID in guardUser.ZoneUIDs)
					{
						var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
							userZones.Add(zoneUID);
					}
					guardUser.ZoneUIDs = userZones;
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
				var copyDevice = new Device();
				//{
				//	UID = currentDevice.UID,
				//	DriverUID = currentDevice.DriverUID,
				//	IntAddress = currentDevice.IntAddress,
				//	Description = currentDevice.Description,
				//	ZoneUID = currentDevice.ZoneUID,
				//	Properties = new List<Property>(currentDevice.Properties),
				//	SystemAUProperties = new List<Property>(currentDevice.SystemAUProperties),
				//	DeviceAUProperties = new List<Property>(currentDevice.DeviceAUProperties)
				//};
				copyDevice.UID = currentDevice.UID;
				copyDevice.DriverUID = currentDevice.DriverUID;
				copyDevice.IntAddress = currentDevice.IntAddress;
				copyDevice.Description = currentDevice.Description;
				copyDevice.ZoneUID = currentDevice.ZoneUID;
				copyDevice.Properties = new List<Property>(currentDevice.Properties);
				if (currentDevice.SystemAUProperties != null)
					copyDevice.SystemAUProperties = new List<Property>(currentDevice.SystemAUProperties);
				if (currentDevice.DeviceAUProperties != null)
					copyDevice.DeviceAUProperties = new List<Property>(currentDevice.DeviceAUProperties);

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
				device.SynchronizeChildern();
			}
		}

		public override bool ValidateVersion()
		{
			var result = true;
			if (RootDevice == null)
			{
				var device = new Device()
				{
					DriverUID = new Guid(DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.Computer).DriverId)
				};
				RootDevice = device;
				result = false;
			}
			foreach (var guardUser in GuardUsers)
			{
				if (guardUser.ZoneUIDs == null)
				{
					guardUser.ZoneUIDs = new List<Guid>();
					result = false;
				}
			}
			foreach (var zone in Zones)
			{
				if (zone.UID == Guid.Empty)
				{
					zone.UID = Guid.NewGuid();
					result = false;
				}
			}
			if (ParameterTemplates == null)
			{
				ParameterTemplates = new List<ParameterTemplate>();
				result = false;
			}

			foreach (var parameterTemplate in ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
					if (deviceParameterTemplate.Device.SystemAUProperties == null)
					{
						deviceParameterTemplate.Device.SystemAUProperties = new List<Property>();
						result = false;
					}
					if (deviceParameterTemplate.Device.DeviceAUProperties == null)
					{
						deviceParameterTemplate.Device.DeviceAUProperties = new List<Property>();
						result = false;
					}
				}
			}

			foreach (var device in RootDevice.GetAllChildren())
			{
				if (device.SystemAUProperties == null)
				{
					device.SystemAUProperties = new List<Property>();
					result = false;
				}
				if (device.DeviceAUProperties == null)
				{
					device.DeviceAUProperties = new List<Property>();
					result = false;
				}
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					if (clause.DeviceUIDs == null)
					{
						clause.DeviceUIDs = new List<Guid>();
						result = false;
					}
				}
			}

			return result;
		}
	}
}