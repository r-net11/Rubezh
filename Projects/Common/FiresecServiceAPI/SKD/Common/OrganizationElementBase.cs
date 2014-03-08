using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganizationElementBase : SKDModelBase
	{
		[DataMember]
		public Guid? OrganizationUid { get; set; }
	}
}
