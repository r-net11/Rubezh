using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganizationFilterBase : FilterBase
	{
		[DataMember]
		public List<Guid> OrganizationUIDs { get; set; }

		public OrganizationFilterBase()
			: base()
		{
			OrganizationUIDs = new List<Guid>();
		}
	}
}
