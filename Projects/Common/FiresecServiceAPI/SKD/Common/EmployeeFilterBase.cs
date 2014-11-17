using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public abstract class EmployeeFilterBase: OrganisationFilterBase
	{
		public EmployeeFilterBase():base()
		{
			EmployeeUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }
	}
}
