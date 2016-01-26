using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKAdditionalState
	{
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}