using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMeasureParameter
	{
		[DataMember]
		public string Dustiness { get; set; }
	}
}