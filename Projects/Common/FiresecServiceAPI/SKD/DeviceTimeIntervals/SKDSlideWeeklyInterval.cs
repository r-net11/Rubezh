using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDSlideWeeklyInterval
	{
		public SKDSlideWeeklyInterval()
		{
			WeeklyIntervalIDs = new List<int>();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<int> WeeklyIntervalIDs { get; set; }

		public void InvalidateWeekIntervals()
		{
			var ids = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Select(item => item.ID).ToList();
			for (int i = 0; i < WeeklyIntervalIDs.Count; i++)
				if (!ids.Contains(WeeklyIntervalIDs[i]))
					WeeklyIntervalIDs[i] = 0;
		}
	}
}