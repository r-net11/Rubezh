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
			SlideWeeklyIntervals = new List<SKDSlideWeekInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			Holidays = new List<SKDHoliday>();
		}

		public List<SKDDevice> Devices { get; set; }
		public List<SKDZone> Zones { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public SKDZone RootZone { get; set; }

		[DataMember]
		public List<NamedSKDTimeInterval> NamedTimeIntervals { get; set; }

		[DataMember]
		public List<SKDSlideDayInterval> SlideDayIntervals { get; set; }

		[DataMember]
		public List<SKDSlideWeekInterval> SlideWeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDHoliday> Holidays { get; set; }

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
			if (SlideWeeklyIntervals == null)
			{
				SlideWeeklyIntervals = new List<SKDSlideWeekInterval>();
				result = false;
			}
			if (WeeklyIntervals == null)
			{
				WeeklyIntervals = new List<SKDWeeklyInterval>();
				result = false;
			}
			foreach (var weeklyInterval in WeeklyIntervals)
			{
				if (weeklyInterval.WeeklyIntervalParts == null)
				{
					weeklyInterval.WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
					result = false;
				}
			}
			foreach (var slideWeeklyInterval in SlideWeeklyIntervals)
			{
				if (slideWeeklyInterval.WeeklyIntervalUIDs == null)
				{
					slideWeeklyInterval.WeeklyIntervalUIDs = new List<Guid>();
					result = false;
				}
			}
			if (Holidays == null)
			{
				Holidays = new List<SKDHoliday>();
				result = false;
			}
			return result;
		}
	}
}