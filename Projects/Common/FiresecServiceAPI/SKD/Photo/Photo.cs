using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class Photo : SKDModelBase
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}