using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDWeeklyInterval
	{
		public SKDWeeklyInterval()
		{
			WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
		}

		public SKDWeeklyInterval(bool createdefault)
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
		public List<SKDWeeklyIntervalPart> WeeklyIntervalParts { get; set; }

		public static List<SKDWeeklyIntervalPart> CreateParts()
		{
			var result = new List<SKDWeeklyIntervalPart>();
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Monday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Tuesday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Wednesday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Thursday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Friday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Saturday });
			result.Add(new SKDWeeklyIntervalPart() { DayOfWeek = SKDDayOfWeek.Sunday });
			return result;
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
			var uids = SKDManager.TimeIntervalsConfiguration.DayIntervals.Select(item => item.UID).ToList();
			WeeklyIntervalParts.ForEach(x =>
			{
				if (!uids.Contains(x.DayIntervalUID))
					x.DayIntervalUID = Guid.Empty;
			});
		}
	}
}