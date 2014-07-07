using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDProperty
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		public SKDDriverProperty DriverProperty { get; set; }
	}
}