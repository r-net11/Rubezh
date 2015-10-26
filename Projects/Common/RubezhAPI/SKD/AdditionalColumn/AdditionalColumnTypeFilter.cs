using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class AdditionalColumnTypeFilter : OrganisationFilterBase
	{
		[DataMember]
		public AdditionalColumnDataType? Type { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }
	}
}