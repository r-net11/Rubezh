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
			TimeIntervalIDs = new List<int>();
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
		public List<int> TimeIntervalIDs { get; set; }

		public void InvalidateDayIntervals()
		{
			var ids = SKDManager.TimeIntervalsConfiguration.TimeIntervals.Select(item => item.ID).ToList();
			for (int i = 0; i < TimeIntervalIDs.Count; i++)
				if (!ids.Contains(TimeIntervalIDs[i]))
					TimeIntervalIDs[i] = 0;
		}
	}
}