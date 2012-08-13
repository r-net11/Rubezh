using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriverTypeMappedProperty
	{
		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public ushort Value { get; set; }
	}
}