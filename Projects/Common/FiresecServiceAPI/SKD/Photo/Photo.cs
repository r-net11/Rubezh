using System.IO;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Photo : SKDIsDeletedModel
	{
		[DataMember]
		public byte[] Data { get; set; }

		public Photo():base() { }
		
		public Photo(Stream stream):base()
		{
			var memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			Data = memoryStream.ToArray();
		}
	}
}
