using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDTimeIntervalPart
	{
		public SKDTimeIntervalPart()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }
	}
}