using Infrustructure.Plans.Devices;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDLibraryState : ILibraryState<SKDLibraryFrame>
	{
		public SKDLibraryState()
		{
			Frames = new List<SKDLibraryFrame>();
			Layer = 0;
		}

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<SKDLibraryFrame> Frames { get; set; }

		[DataMember]
		public int Layer { get; set; }

		#region ILibraryState<SKDLibraryFrame> Members

		XStateClass ILibraryState<SKDLibraryFrame>.StateType
		{
			get { return StateClass; }
		}

		#endregion ILibraryState<SKDLibraryFrame> Members
	}
}