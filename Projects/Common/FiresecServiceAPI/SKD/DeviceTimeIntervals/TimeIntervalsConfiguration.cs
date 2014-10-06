using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	public class TimeIntervalsConfiguration : VersionedConfiguration
	{
		public TimeIntervalsConfiguration()
		{
			DayIntervals = new List<SKDDayInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
			Holidays = new List<SKDHoliday>();
		}

		[DataMember]
		public List<SKDDayInterval> DayIntervals { get; set; }

		[DataMember]
		public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDSlideDayInterval> SlideDayIntervals { get; set; }

		[DataMember]
		public List<SKDSlideWeeklyInterval> SlideWeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDHoliday> Holidays { get; set; }

		public bool ValidateIntervals()
		{
			var result = true;

			if (WeeklyIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;
			if (SlideWeeklyIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;
			if (SlideDayIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;

			WeeklyIntervals = WeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			SlideWeeklyIntervals = SlideWeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			SlideDayIntervals = SlideDayIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();

			foreach (var weeklyInterval in WeeklyIntervals)
				if (weeklyInterval.WeeklyIntervalParts == null)
				{
					weeklyInterval.WeeklyIntervalParts = SKDWeeklyInterval.CreateParts();
					result = false;
				}
			foreach (var slideWeeklyInterval in SlideWeeklyIntervals)
				if (slideWeeklyInterval.WeeklyIntervalIDs.RemoveAll(id => id < 0 || id > 128) > 0)
					result = false;
			foreach (var slideDayInterval in SlideDayIntervals)
				if (slideDayInterval.DayIntervalIDs.RemoveAll(id => id < 0 || id > 128) > 0)
					result = false;

			var neverDayInterval = DayIntervals.FirstOrDefault(x => x.Name == "<Никогда>");
			if (neverDayInterval == null)
			{
				neverDayInterval = new SKDDayInterval();
				neverDayInterval.Name = "<Никогда>";
				DayIntervals.Add(neverDayInterval);
			}

			var alwaysDayInterval = DayIntervals.FirstOrDefault(x => x.Name == "<Всегда>");
			if (alwaysDayInterval == null)
			{
				alwaysDayInterval = new SKDDayInterval();
				alwaysDayInterval.Name = "<Всегда>";
				alwaysDayInterval.DayIntervalParts.Add(new SKDDayIntervalPart() { StartMilliseconds = 0, EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds });
				DayIntervals.Add(alwaysDayInterval);
			}

			if (WeeklyIntervals.Count == 0)
			{
				var neverWeeklyInterval = new SKDWeeklyInterval(true);
				neverWeeklyInterval.Name = "<Никогда>";
				neverWeeklyInterval.ID = 0;
				foreach (var weeklyIntervalPart in neverWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.DayIntervalUID = neverDayInterval.UID;
				}
				WeeklyIntervals.Add(neverWeeklyInterval);

				var alwaysWeeklyInterval = new SKDWeeklyInterval(true);
				alwaysWeeklyInterval.Name = "<Всегда>";
				alwaysWeeklyInterval.ID = 1;
				foreach (var weeklyIntervalPart in alwaysWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.DayIntervalUID = alwaysDayInterval.UID;
				}
				WeeklyIntervals.Add(alwaysWeeklyInterval);
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