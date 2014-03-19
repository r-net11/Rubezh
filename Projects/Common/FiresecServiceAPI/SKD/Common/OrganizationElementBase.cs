using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganizationElementBase : SKDIsDeletedModel
	{
		[DataMember]
		public Guid? OrganizationUID { get; set; }
	}
}