using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace XFiresecAPI
{
	[DataContract]
	public class SKDLibraryState : ILibraryState<SKDLibraryFrame>
	{
		public SKDLibraryState()
		{
			Frames = new List<SKDLibraryFrame>();
			Frames.Add(new SKDLibraryFrame() { Id = 0 });
			Layer = 0;
		}

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public List<SKDLibraryFrame> Frames { get; set; }

		[DataMember]
		public int Layer { get; set; }
	}
}