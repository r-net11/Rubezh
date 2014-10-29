using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class GKCardFilter : OrganisationFilterBase
	{
		public GKCardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }
	}
}