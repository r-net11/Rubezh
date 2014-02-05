using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI;

namespace XFiresecAPI
{
	[DataContract]
	public class SKDConfiguration : VersionedConfiguration
	{
		public SKDConfiguration()
		{
			TimeIntervals = new List<SKDTimeInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			SlideWeeklyIntervals = new List<SKDSlideWeekInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			Holidays = new List<SKDHoliday>();
			SKDSystemConfiguration = new SKDSystemConfiguration();
		}

		public List<SKDDevice> Devices { get; set; }
		public List<SKDZone> Zones { get; set; }

		[DataMember]
		public SKDDevice RootDevice { get; set; }

		[DataMember]
		public SKDZone RootZone { get; set; }

		[DataMember]
		public List<SKDTimeInterval> TimeIntervals { get; set; }

		[DataMember]
		public List<SKDSlideDayInterval> SlideDayIntervals { get; set; }

		[DataMember]
		public List<SKDSlideWeekInterval> SlideWeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDHoliday> Holidays { get; set; }

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

		public override bool ValidateVersion()
		{
			var result = true;
			if (TimeIntervals == null)
			{
				TimeIntervals = new List<SKDTimeInterval>();
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

			var alwaysTimeInterval = TimeIntervals.FirstOrDefault(x => x.Name == "Всегда" && x.IsDefault);
			if (alwaysTimeInterval == null)
			{
				alwaysTimeInterval = new SKDTimeInterval() { Name = "Всегда", IsDefault = true };
				alwaysTimeInterval.TimeIntervalParts.Add(new SKDTimeIntervalPart() { StartTime = DateTime.MinValue, EndTime = DateTime.MinValue.AddHours(23).AddMinutes(59) });
				TimeIntervals.Add(alwaysTimeInterval);
				result = false;
			}

			var neverTimeInterval = TimeIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverTimeInterval == null)
			{
				neverTimeInterval = new SKDTimeInterval() { Name = "Никогда", IsDefault = true };
				neverTimeInterval.TimeIntervalParts.Add(new SKDTimeIntervalPart() { StartTime = DateTime.MinValue, EndTime = DateTime.MinValue });
				TimeIntervals.Add(neverTimeInterval);
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
			if (SKDSystemConfiguration == null)
			{
				SKDSystemConfiguration = new SKDSystemConfiguration();
				result = false;
			}

			return result;
		}
	}
}