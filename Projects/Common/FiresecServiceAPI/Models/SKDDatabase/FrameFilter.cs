using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

		public FrameFilter()
		{
			Uids = new List<Guid>();
			CameraUid = new List<Guid>();
			JournalItemUid = new List<Guid>();
		}		


	}
}