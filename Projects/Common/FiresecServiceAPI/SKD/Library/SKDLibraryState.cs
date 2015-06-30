using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using Infrustructure.Plans.Devices;

namespace FiresecAPI.SKD
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

		#endregion
	}
}