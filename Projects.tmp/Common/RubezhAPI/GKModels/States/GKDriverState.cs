using System.Runtime.Serialization;

namespace RubezhAPI.GK
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