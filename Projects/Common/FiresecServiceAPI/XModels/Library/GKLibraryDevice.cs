using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKLibraryDevice : ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>
	{
		public GKLibraryDevice()
		{
			UID = Guid.NewGuid();
			IsAlternative = false;
			States = new List<GKLibraryState>();
		}

		[XmlIgnore]
		public GKDriver Driver { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public bool IsAlternative { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

		[DataMember]
		public List<GKLibraryState> States { get; set; }

		#region ILibraryDevice<XStateClass,LibraryXFrame,GKState> Members

		[XmlIgnore]
		Guid ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>.DriverId
		{
			get { return DriverUID; }
			set { DriverUID = value; }
		}

		[XmlIgnore]
		List<GKLibraryState> ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>.States
		{
			get { return States; }
		}

		#endregion
	}
}