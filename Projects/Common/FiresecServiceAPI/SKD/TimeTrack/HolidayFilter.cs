using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class HolidayFilter : OrganisationFilterBase
	{
		[DataMember]
		public int Year { get; set; }
	}
}