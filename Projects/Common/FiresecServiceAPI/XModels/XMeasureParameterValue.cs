using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XMeasureParameterValue
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public double Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }
	}
}