using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDSlideDayInterval
	{
		public SKDSlideDayInterval()
		{
			DayIntervalIDs = new List<int>();
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
		public List<int> DayIntervalIDs { get; set; }

		public void InvalidateDayIntervals()
		{
			var nos = SKDManager.TimeIntervalsConfiguration.DayIntervals.Select(item => item.No).ToList();
			for (int i = 0; i < DayIntervalIDs.Count; i++)
				if (!nos.Contains(DayIntervalIDs[i]))
					DayIntervalIDs[i] = 0;
		}
	}
}