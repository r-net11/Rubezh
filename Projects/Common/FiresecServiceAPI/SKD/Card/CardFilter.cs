using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardFilter : OrganisationFilterBase
	{
		public CardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }
	}
}