using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class OrganisationFilter : IsDeletedFilter
	{
		[DataMember]
		public User User { get; set; }
	}
}