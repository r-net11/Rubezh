using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Department
	{
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public Guid? ParentDepartmentUid { get; set; }
		[DataMember]
		public List<Guid> ChildDepartmentUids { get; set; }
		[DataMember]
		public Guid? ContactEmployeeUid { get; set; }
		[DataMember]
		public Guid? AttendantEmployeeUId { get; set; }
		[DataMember]
		public List<Guid> PhoneUids { get; set; }
	}
}
