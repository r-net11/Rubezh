using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDWeeklyInterval
	{
		public SKDWeeklyInterval()
		{
			UID = Guid.NewGuid();
			WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
			for (int i = 1; i <= 7; i++)
			{
				WeeklyIntervalParts.Add(new SKDWeeklyIntervalPart() { No = i, IsHolliday = false });
			}
			for (int i = 1; i <= 8; i++)
			{
				WeeklyIntervalParts.Add(new SKDWeeklyIntervalPart() { No = i, IsHolliday = true });
			}
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<SKDWeeklyIntervalPart> WeeklyIntervalParts { get; set; }
	}
}