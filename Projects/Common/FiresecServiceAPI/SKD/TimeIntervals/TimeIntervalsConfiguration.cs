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
			TimeIntervals = new List<SKDTimeInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			Holidays = new List<SKDHoliday>();
		}

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

		public bool ValidateIntervals()
		{
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
			if (TimeIntervals.RemoveAll(item => item.ID < 0 || item.ID > 127) > 0)
				result = false;
			if (WeeklyIntervals.RemoveAll(item => item.ID < 0 || item.ID > 127) > 0)
				result = false;
			if (SlideWeeklyIntervals.RemoveAll(item => item.ID < 0 || item.ID > 127) > 0)
				result = false;
			if (SlideDayIntervals.RemoveAll(item => item.ID < 0 || item.ID > 127) > 0)
				result = false;
			TimeIntervals = TimeIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			WeeklyIntervals = WeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			SlideWeeklyIntervals = SlideWeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			SlideDayIntervals = SlideDayIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			foreach (var weeklyInterval in WeeklyIntervals)
				if (weeklyInterval.WeeklyIntervalParts == null)
				{
					weeklyInterval.WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
					result = false;
				}
			foreach (var slideWeeklyInterval in SlideWeeklyIntervals)
				if (slideWeeklyInterval.WeeklyIntervalIDs == null)
				{
					slideWeeklyInterval.WeeklyIntervalIDs = new List<int>();
					result = false;
				}
				else if (slideWeeklyInterval.WeeklyIntervalIDs.RemoveAll(id => id < 0 || id > 127) > 0)
					result = false;
			foreach (var slideDayInterval in SlideDayIntervals)
				if (slideDayInterval.TimeIntervalIDs == null)
				{
					slideDayInterval.TimeIntervalIDs = new List<int>();
					result = false;
				}
				else if (slideDayInterval.TimeIntervalIDs.RemoveAll(id => id < 0 || id > 127) > 0)
					result = false;

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