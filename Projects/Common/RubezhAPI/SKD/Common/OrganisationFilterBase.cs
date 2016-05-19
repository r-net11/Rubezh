using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public abstract class OrganisationFilterBase : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> OrganisationUIDs { get; set; }

		[DataMember]
		public User User { get; set; }

		public OrganisationFilterBase()
			: base()
		{
			OrganisationUIDs = new List<Guid>();
		}
	}
}