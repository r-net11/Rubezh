using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XDriverState
	{
		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public XStateClass XStateClass { get; set; }
	}
}