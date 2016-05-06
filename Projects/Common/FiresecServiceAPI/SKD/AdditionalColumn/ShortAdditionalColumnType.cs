using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ShortAdditionalColumnType : IOrganisationElement
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public AdditionalColumnDataType DataType { get; set; }

		[DataMember]
		public bool IsInGrid { get; set; }

		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }
	}
}