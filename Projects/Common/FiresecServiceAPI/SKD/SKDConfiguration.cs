using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDConfiguration : VersionedConfiguration
	{
		public SKDConfiguration()
		{
			TimeIntervalsConfiguration = new TimeIntervalsConfiguration();
			SKDSystemConfiguration = new SKDSystemConfiguration();
		}

		public List<SKDDevice> Devices { get; set; }
		public List<SKDZone> Zones { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public SKDZone RootZone { get; set; }

		[DataMember]
		public TimeIntervalsConfiguration TimeIntervalsConfiguration { get; set; }

		[DataMember]
		public SKDSystemConfiguration SKDSystemConfiguration { get; set; }

		public void Update()
		{
			Devices = new List<SKDDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChildDevice(RootDevice);
			}

			Zones = new List<SKDZone>();
			if (RootZone != null)
			{
				RootZone.Parent = null;
				Zones.Add(RootZone);
				AddChildZone(RootZone);
			}
		}

		void AddChildDevice(SKDDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChildDevice(device);
			}
		}

		void AddChildZone(SKDZone parentZone)
		{
			foreach (var zone in parentZone.Children)
			{
				zone.Parent = parentZone;
				Zones.Add(zone);
				AddChildZone(zone);
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
				{
					var children = new List<SKDDevice>();
					foreach (var childDevice in device.Children)
					{
						if (childDevice.DriverType == SKDDriverType.Reader)
						{
							children.Add(childDevice);
							childDevice.IntAddress = children.Count;
							childDevice.Address = childDevice.IntAddress.ToString();
						}
					}
					foreach (var childDevice in device.Children)
					{
						if (childDevice.DriverType != SKDDriverType.Reader)
						{
							children.Add(childDevice);
							childDevice.IntAddress = children.Count;
							childDevice.Address = childDevice.IntAddress.ToString();
						}
					}
					device.Children = children;
				}
			}
		}

		public override bool ValidateVersion()
		{
			var result = true;
			if (TimeIntervalsConfiguration == null)
			{
				TimeIntervalsConfiguration = new TimeIntervalsConfiguration();
				result = false;
			}
			result &= TimeIntervalsConfiguration.ValidateIntervals();

			if (SKDSystemConfiguration == null)
			{
				SKDSystemConfiguration = new SKDSystemConfiguration();
				result = false;
			}

			return result;
		}

		public bool ValidateIntervals()
		{
			var result = true;
			result &= TimeIntervalsConfiguration.ValidateIntervals();
			return result;
		}
	}
}