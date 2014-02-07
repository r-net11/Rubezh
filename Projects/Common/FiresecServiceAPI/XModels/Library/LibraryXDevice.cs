using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

namespace XFiresecAPI
{
	[DataContract]
	public class LibraryXDevice : ILibraryDevice<LibraryXState, LibraryXFrame, XStateClass>
	{
		public LibraryXDevice()
		{
			UID = Guid.NewGuid();
			IsAlternative = false;
			XStates = new List<LibraryXState>();
			var libraryXState = new LibraryXState()
			{
				XStateClass = XStateClass.No,
			};
			XStates.Add(libraryXState);
		}

		public XDriver Driver { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid XDriverId { get; set; }

		[DataMember]
		public bool IsAlternative { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

		[DataMember]
		public List<LibraryXState> XStates { get; set; }

		#region ILibraryDevice<XStateClass,LibraryXFrame,XState> Members

		Guid ILibraryDevice<LibraryXState, LibraryXFrame, XStateClass>.DriverId
		{
			get { return XDriverId; }
			set { XDriverId = value; }
		}

		List<LibraryXState> ILibraryDevice<LibraryXState, LibraryXFrame, XStateClass>.States
		{
			get { return XStates; }
		}

		#endregion
	}
}