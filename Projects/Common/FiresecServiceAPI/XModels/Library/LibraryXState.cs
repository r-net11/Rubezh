using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace XFiresecAPI
{
	[DataContract]
	public class LibraryXState : ILibraryState<LibraryXFrame, XStateClass>
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
		public List<LibraryXFrame> XFrames { get; set; }

		[DataMember]
		public int Layer { get; set; }

		#region ILibraryState<SKDLibraryFrame,XStateClass> Members

		List<LibraryXFrame> ILibraryState<LibraryXFrame, XStateClass>.Frames
		{
			get { return XFrames; }
			set { XFrames = value; }
		}

		XStateClass ILibraryState<LibraryXFrame, XStateClass>.StateType
		{
			get { return XStateClass; }
		}

		#endregion
	}
}