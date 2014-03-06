using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Phone : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NumberString { get; set; }

		[DataMember]
		public Guid? DepartmentUid { get; set; }
	}
}