using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKMeasureParameterValue
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string StringValue { get; set; }
	}
}