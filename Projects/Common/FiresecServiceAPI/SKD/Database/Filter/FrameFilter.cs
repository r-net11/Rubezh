using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public class FrameFilter : FilterBase
	{
		[DataMember]
		public List<Guid> Uids { get; set; }

		[DataMember]
		public List<Guid> CameraUid { get; set; }

		[DataMember]
		public List<Guid> JournalItemUid { get; set; }

		[DataMember]
		public DateTimePeriod DateTime { get; set; }

		public FrameFilter()
		{
			Uids = new List<Guid>();
			CameraUid = new List<Guid>();
			JournalItemUid = new List<Guid>();
		}		


	}
}