using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKLibraryState : ILibraryState<GKLibraryFrame, XStateClass>
	{
		public GKLibraryState()
		{
			Frames = new List<GKLibraryFrame>();
			Layer = 0;
		}

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<GKLibraryFrame> Frames { get; set; }

		[DataMember]
		public int Layer { get; set; }

		#region ILibraryState<SKDLibraryFrame,XStateClass> Members

		List<GKLibraryFrame> ILibraryState<GKLibraryFrame, XStateClass>.Frames
		{
			get { return Frames; }
			set { Frames = value; }
		}

		XStateClass ILibraryState<GKLibraryFrame, XStateClass>.StateType
		{
			get { return StateClass; }
		}

		#endregion
	}
}