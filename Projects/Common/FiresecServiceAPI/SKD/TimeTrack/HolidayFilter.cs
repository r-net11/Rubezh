using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class HolidayFilter : OrganisationFilterBase
	{
		[DataMember]
		public int Year { get; set; }
	}
}