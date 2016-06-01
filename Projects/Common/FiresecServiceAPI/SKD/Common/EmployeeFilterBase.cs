using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class EmployeeFilterBase : OrganisationFilterBase
	{
		protected EmployeeFilterBase()
		{
			EmployeeUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }
	}
}