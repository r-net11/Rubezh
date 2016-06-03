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
			WeeklyIntervalParts = CreateParts(); //TODO: can move to default constructor.
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
			var result = new List<SKDWeeklyIntervalPart>
			{
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Monday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Tuesday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Wednesday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Thursday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Friday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Saturday},
				new SKDWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Sunday}
			};
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