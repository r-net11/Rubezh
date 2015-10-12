using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public abstract class OrganisationElementBase : SKDIsDeletedModel
	{
		[DataMember]
		public Guid OrganisationUID { get; set; }
	}
}