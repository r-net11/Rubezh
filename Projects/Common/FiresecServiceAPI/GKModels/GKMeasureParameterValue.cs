using System.Runtime.Serialization;

namespace FiresecAPI.GK
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