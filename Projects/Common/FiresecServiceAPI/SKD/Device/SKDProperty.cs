using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.SKD
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

		[XmlIgnore]
		public SKDDriverProperty DriverProperty { get; set; }
	}
}