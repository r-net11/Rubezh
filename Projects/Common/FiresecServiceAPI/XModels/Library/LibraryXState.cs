using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace FiresecAPI.GK
{
	public class LibraryXState : ILibraryState<LibraryXFrame, XStateClass>
	{
		public LibraryXState()
		{
			XFrames = new List<LibraryXFrame>();
			Layer = 0;
		}

		public XStateClass XStateClass { get; set; }
		public List<LibraryXFrame> XFrames { get; set; }
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