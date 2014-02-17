using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Frame : SKDModelBase
	{
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