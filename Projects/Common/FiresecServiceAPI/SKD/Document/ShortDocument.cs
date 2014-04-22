using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class ShortDocument
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid? OrganisationUID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public DateTime IssueDate { get; set; }

		[DataMember]
		public DateTime LaunchDate { get; set; }
	}
}
