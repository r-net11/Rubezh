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
			SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
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
		public List<SKDSlideWeeklyInterval> SlideWeeklyIntervals { get; set; }

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
			if(!ValidateIntervals())
				result = false;

			if (SKDSystemConfiguration == null)
			{
				SKDSystemConfiguration = new SKDSystemConfiguration();
				result = false;
			}

			return result;
		}

		public bool ValidateIntervals()
		{
			TimeIntervals = new List<SKDTimeInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
			Holidays = new List<SKDHoliday>();

			var result = true;
			if (TimeIntervals == null)
			{
				TimeIntervals = new List<SKDTimeInterval>();
				result = false;
			}
			if (WeeklyIntervals == null)
			{
				WeeklyIntervals = new List<SKDWeeklyInterval>();
				result = false;
			}
			if (SlideDayIntervals == null)
			{
				SlideDayIntervals = new List<SKDSlideDayInterval>();
				result = false;
			}
			if (SlideWeeklyIntervals == null)
			{
				SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
				result = false;
			}
			if (Holidays == null)
			{
				Holidays = new List<SKDHoliday>();
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

			var alwaysWeeklyInterval = WeeklyIntervals.FirstOrDefault(x => x.Name == "Доступ разрешен" && x.IsDefault);
			if (alwaysWeeklyInterval == null)
			{
				alwaysWeeklyInterval = new SKDWeeklyInterval() { Name = "Доступ разрешен", IsDefault = true };
				foreach (var weeklyIntervalPart in alwaysWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.TimeIntervalUID = alwaysTimeInterval.UID;
				}
				WeeklyIntervals.Add(alwaysWeeklyInterval);
				result = false;
			}

			var neverWeeklyInterval = WeeklyIntervals.FirstOrDefault(x => x.Name == "Доступ запрещен" && x.IsDefault);
			if (neverWeeklyInterval == null)
			{
				neverWeeklyInterval = new SKDWeeklyInterval() { Name = "Доступ запрещен", IsDefault = true };
				foreach (var weeklyIntervalPart in neverWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.TimeIntervalUID = neverTimeInterval.UID;
				}
				WeeklyIntervals.Add(neverWeeklyInterval);
				result = false;
			}

			var neverSlideDayInterval = SlideDayIntervals.FirstOrDefault(x => x.Name == "Доступ запрещен" && x.IsDefault);
			if (neverSlideDayInterval == null)
			{
				neverSlideDayInterval = new SKDSlideDayInterval() { Name = "Доступ запрещен", IsDefault = true };
				neverSlideDayInterval.TimeIntervalUIDs.Add(neverTimeInterval.UID);
				SlideDayIntervals.Add(neverSlideDayInterval);
				result = false;
			}

			var neverSlideWeeklyInterval = SlideWeeklyIntervals.FirstOrDefault(x => x.Name == "Доступ запрещен" && x.IsDefault);
			if (neverSlideWeeklyInterval == null)
			{
				neverSlideWeeklyInterval = new SKDSlideWeeklyInterval() { Name = "Доступ запрещен", IsDefault = true };
				neverSlideWeeklyInterval.WeeklyIntervalUIDs.Add(neverWeeklyInterval.UID);
				SlideWeeklyIntervals.Add(neverSlideWeeklyInterval);
				result = false;
			}

			if (Holidays.Count == 0)
			{
				Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 1) });
				Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 2) });
				Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 3) });
				Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 4) });
				Holidays.Add(new SKDHoliday() { Name = "Рождество", DateTime = new DateTime(2000, 1, 7) });
				Holidays.Add(new SKDHoliday() { Name = "День советской армии и военно-морского флота", DateTime = new DateTime(2000, 2, 23) });
				Holidays.Add(new SKDHoliday() { Name = "Международный женский день", DateTime = new DateTime(2000, 3, 8) });
				Holidays.Add(new SKDHoliday() { Name = "День победы", DateTime = new DateTime(2000, 5, 9) });
				Holidays.Add(new SKDHoliday() { Name = "День России", DateTime = new DateTime(2000, 6, 12) });
				Holidays.Add(new SKDHoliday() { Name = "День примерения", DateTime = new DateTime(2000, 11, 4) });
				Holidays.Add(new SKDHoliday() { Name = "Новый год", DateTime = new DateTime(2000, 12, 31) });
				result = false;
			}

			return result;
		}
	}
}