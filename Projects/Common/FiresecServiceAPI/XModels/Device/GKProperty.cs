using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKProperty
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		public GKDriverProperty DriverProperty { get; set; }
	}
}