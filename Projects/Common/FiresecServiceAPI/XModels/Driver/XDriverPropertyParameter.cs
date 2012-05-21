using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriverPropertyParameter
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

		[DataMember]
		public short Value { get; set; }
	}
}