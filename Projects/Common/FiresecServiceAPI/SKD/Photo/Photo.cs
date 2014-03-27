using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Photo : SKDIsDeletedModel
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}
