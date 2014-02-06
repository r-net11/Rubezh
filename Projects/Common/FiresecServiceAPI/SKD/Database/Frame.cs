using System;
using System.Runtime.Serialization;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace FiresecAPI
{
	[DataContract]
	public class Frame
	{
		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid? CameraUid { get; set; }

		[DataMember]
		public Guid? JournalItemUid { get; set; }

		[DataMember]
		public DateTime? DateTime { get; set; }

		[DataMember]
		public byte[] FrameData { get; set; }

		public Image Image
		{
			get
			{
				var stream = new MemoryStream(FrameData);
				return Image.FromStream(stream);
			}
			set
			{
				using (var stream = new MemoryStream(FrameData))
				{
					value.Save(stream, ImageFormat.Jpeg);
				}
			}
		}
	}
}
