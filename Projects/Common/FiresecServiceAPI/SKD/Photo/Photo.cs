using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Photo : SKDModelBase
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}
