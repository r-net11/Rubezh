using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDConfiguration : VersionedConfiguration
	{
		public SKDConfiguration()
		{
			Devices = new List<SKDDevice>();
			Zones = new List<SKDZone>();
			Doors = new List<SKDDoor>();
			TimeIntervalsConfiguration = new TimeIntervalsConfiguration();
		}

		[XmlIgnore]
		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public List<SKDZone> Zones { get; set; }

		[DataMember]
		public List<SKDDoor> Doors { get; set; }

		[DataMember]
		public TimeIntervalsConfiguration TimeIntervalsConfiguration { get; set; }

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

		private void AddChildDevice(SKDDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChildDevice(device);
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
			result &= TimeIntervalsConfiguration.Validate();

			if (Doors == null)
			{
				Doors = new List<SKDDoor>();
				result = false;
			}

			if (Zones == null)
			{
				Zones = new List<SKDZone>();
				result = false;
			}

			return result;
		}
	}
}