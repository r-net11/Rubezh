using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace XFiresecAPI
{
	[DataContract]
	public class LibraryXState : ILibraryState<LibraryXFrame>
	{
		public LibraryXState()
		{
			XFrames = new List<LibraryXFrame>();
			XFrames.Add(new LibraryXFrame() { Id = 0 });
			Layer = 0;
		}

		[DataMember]
		public XStateClass XStateClass { get; set; }

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public List<LibraryXFrame> XFrames { get; set; }

		[DataMember]
		public int Layer { get; set; }

		#region ILibraryState<LibraryXFrame> Members

		List<LibraryXFrame> ILibraryState<LibraryXFrame>.Frames
		{
			get { return XFrames; }
			set { XFrames = value; }
		}

		#endregion
	}
}