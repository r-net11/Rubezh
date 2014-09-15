using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDayIntervalPart
	{
		public SKDDayIntervalPart()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }
	}
}