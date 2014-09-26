using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Property
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }

		[XmlIgnore]
		public DriverProperty DriverProperty { get; set; }
	}
}