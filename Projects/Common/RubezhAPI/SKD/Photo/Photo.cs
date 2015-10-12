using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class Photo : SKDModelBase
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}