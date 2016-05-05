using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class OrganisationElementBase : SKDIsDeletedModel
	{
		[DataMember]
		public Guid OrganisationUID { get; set; }
	}
}