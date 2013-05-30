using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XAUParameter
	{
		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsDelay { get; set; }

		[DataMember]
		public string InternalName { get; set; }
	}
}