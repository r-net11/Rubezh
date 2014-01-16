	using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class SKDConfiguration : VersionedConfiguration
	{
		public SKDConfiguration()
		{
			NamedTimeIntervals = new List<NamedSKDTimeInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
		}

		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public List<NamedSKDTimeInterval> NamedTimeIntervals { get; set; }

		[DataMember]
		public List<SKDSlideDayInterval> SlideDayIntervals { get; set; }

		[DataMember]
		public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }

		public void Update()
		{
			Devices = new List<SKDDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(SKDDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		public override bool ValidateVersion()
		{
			var result = true;
			if (NamedTimeIntervals == null)
			{
				NamedTimeIntervals = new List<NamedSKDTimeInterval>();
				result = false;
			}
			if (SlideDayIntervals == null)
			{
				SlideDayIntervals = new List<SKDSlideDayInterval>();
				result = false;
			}
			if (WeeklyIntervals == null)
			{
				WeeklyIntervals = new List<SKDWeeklyInterval>();
				result = false;
			}
			return result;
		}
	}
}