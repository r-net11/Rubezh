using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDWeeklyIntervalPart
	{
		public SKDWeeklyIntervalPart()
		{

		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public bool IsHolliday { get; set; }

		[DataMember]
		public Guid TimeIntervalUID { get; set; }
	}
}