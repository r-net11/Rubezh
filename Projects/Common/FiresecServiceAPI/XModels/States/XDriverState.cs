using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriverState
	{
		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public XStateType XStateClass { get; set; }
	}
}