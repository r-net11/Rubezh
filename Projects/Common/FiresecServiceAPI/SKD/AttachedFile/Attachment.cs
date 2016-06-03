using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class Attachment : SKDModelBase
	{
		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public byte[] Data { get; set; }
	}
}
