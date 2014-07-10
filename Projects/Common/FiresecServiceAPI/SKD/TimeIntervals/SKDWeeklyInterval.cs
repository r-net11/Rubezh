using System;
using Common;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyInterval
	{
		public SKDWeeklyInterval()
		{
			WeeklyIntervalParts = CreateParts();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ReadOnlyCollection<SKDWeeklyIntervalPart> WeeklyIntervalParts { get; set; }

		public static ReadOnlyCollection<SKDWeeklyIntervalPart> CreateParts()
		{
			var list = new List<SKDWeeklyIntervalPart>();
			for (int i = 1; i <= 7; i++)
				list.Add(new SKDWeeklyIntervalPart() { No = i, IsHolliday = false, TimeIntervalID = 0 });
			for (int i = 1; i <= 8; i++)
				list.Add(new SKDWeeklyIntervalPart() { No = i, IsHolliday = true, TimeIntervalID = 0 });
			return new ReadOnlyCollection<SKDWeeklyIntervalPart>(list);
		}

		public override bool Equals(object obj)
		{
			if (obj is SKDWeeklyInterval)
				return ((SKDWeeklyInterval)obj).ID == ID;
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}
		public void InvalidateDayIntervals()
		{
			var ids = SKDManager.TimeIntervalsConfiguration.TimeIntervals.Select(item => item.ID).ToList();
			WeeklyIntervalParts.Where(part => !part.IsHolliday).ForEach(part =>
			{
				if (!ids.Contains(part.TimeIntervalID))
					part.TimeIntervalID = 0;
			});
		}
		public void InvalidateHolidays()
		{
			var uids = SKDManager.TimeIntervalsConfiguration.Holidays.Select(item => item.UID).ToList();
			WeeklyIntervalParts.Where(part => part.IsHolliday).ForEach(part =>
			{
				if (!uids.Contains(part.HolidayUID))
					part.HolidayUID = Guid.Empty;
			});
		}
	}
}