using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class OrganisationFilterBase : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> OrganisationUIDs { get; set; }

		[DataMember]
		public Guid UserUID { get; set; }

		protected OrganisationFilterBase()
		{
			OrganisationUIDs = new List<Guid>();
		}
	}
}