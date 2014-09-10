using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
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
			var ids = SKDManager.TimeIntervalsConfiguration.DayIntervals.Select(item => item.ID).ToList();
			for (int i = 0; i < DayIntervalIDs.Count; i++)
				if (!ids.Contains(DayIntervalIDs[i]))
					DayIntervalIDs[i] = 0;
		}
	}
}