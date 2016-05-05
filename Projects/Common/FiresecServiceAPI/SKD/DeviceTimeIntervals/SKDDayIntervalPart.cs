using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
		public double StartMilliseconds { get; set; }

		[DataMember]
		public double EndMilliseconds { get; set; }
	}
}