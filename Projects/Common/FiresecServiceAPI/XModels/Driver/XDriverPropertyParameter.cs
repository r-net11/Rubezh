using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriverPropertyParameter
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort Value { get; set; }
	}
}