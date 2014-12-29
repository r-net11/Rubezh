using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDriverState
	{
		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public XStateClass XStateClass { get; set; }
	}
}