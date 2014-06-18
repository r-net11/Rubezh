using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDConfiguration : VersionedConfiguration
	{
		public SKDConfiguration()
		{
			Doors = new List<Door>();
			Zones = new List<SKDZone>();
			TimeIntervalsConfiguration = new TimeIntervalsConfiguration();
			SKDSystemConfiguration = new SKDSystemConfiguration();
		}

		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public List<SKDZone> Zones { get; set; }

		[DataMember]
		public List<Door> Doors { get; set; }

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

			if (Doors == null)
			{
				Doors = new List<Door>();
				result = false;
			}

			if (Zones == null)
			{
				Zones = new List<SKDZone>();
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