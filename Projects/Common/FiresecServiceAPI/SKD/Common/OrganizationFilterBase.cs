using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganizationFilterBase:FilterBase
	{
		[DataMember]
		public List<Guid> OrganizationUids { get; set; }

		public OrganizationFilterBase():base()
		{
			OrganizationUids = new List<Guid>();
		}
	}
}
