using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XProperty
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		public XDriverProperty DriverProperty { get; set; }
	}
}