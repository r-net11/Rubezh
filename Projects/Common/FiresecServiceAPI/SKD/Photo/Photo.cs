using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Photo : SKDModelBase
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}