using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganisationElementBase : SKDIsDeletedModel
	{
		[DataMember]
		public Guid? OrganisationUID { get; set; }
	}
}