using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using Infrustructure.Plans.Devices;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDLibraryDevice : ILibraryDevice<SKDLibraryState, SKDLibraryFrame, XStateClass>
	{
		public SKDLibraryDevice()
		{
			UID = Guid.NewGuid();
			IsAlternative = false;
			States = new List<SKDLibraryState>();
			var libraryXState = new SKDLibraryState()
			{
				StateClass = XStateClass.No,
			};
			States.Add(libraryXState);
		}

		public SKDDriver Driver { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverId { get; set; }

		[DataMember]
		public bool IsAlternative { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

		[DataMember]
		public List<SKDLibraryState> States { get; set; }
	}
}