using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

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
	}
}
