using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class HolidayFilter : OrganisationFilterBase
	{
		[DataMember]
		public int Year { get; set; }
	}
}