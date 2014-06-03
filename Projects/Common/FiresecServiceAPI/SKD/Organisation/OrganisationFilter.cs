using System;
using System.Runtime.Serialization;
namespace FiresecAPI.SKD
{
	[DataContract]
	public class OrganisationFilter : IsDeletedFilter
	{
		[DataMember]
		public Guid? UserUID { get; set; }
	}
}