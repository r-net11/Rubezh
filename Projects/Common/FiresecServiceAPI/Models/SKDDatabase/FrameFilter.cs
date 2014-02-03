using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public class FrameFilter
	{
		[DataMember]
		public List<Guid> Uids { get; set; }

		[DataMember]
		public List<Guid> CameraUid { get; set; }

		[DataMember]
		public List<Guid> JournalItemUid { get; set; }

		[DataMember]
		public DateTimePeriod DateTime { get; set; }
	}
}