using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDCard
	{
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public int? Series { get; set; }
		[DataMember]
		public int? Number { get; set; }
		[DataMember]
		public Guid? EmployeeUid { get; set; }
		[DataMember]
		public DateTime? ValidFrom { get; set; }
		[DataMember]
		public DateTime? ValidTo { get; set; }
		[DataMember]
		public List<Guid> ZoneLinkUids { get; set; }
		[DataMember]
		public bool? IsAntipass { get; set; }
		[DataMember]
		public bool? IsInStopList { get; set; }
		[DataMember]
		public string StopReason { get; set; }
	}
}
