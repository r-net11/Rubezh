using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class LibraryDevice : ILibraryDevice<LibraryState, LibraryFrame, StateType>
	{
		public LibraryDevice()
		{
			UID = Guid.NewGuid();
			IsAlternative = false;
			States = new List<LibraryState>();
			Presenters = new List<LibraryDevicePresenter>();
		}

		[XmlIgnore]
		public Driver Driver { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverId { get; set; }

		[DataMember]
		public bool IsAlternative { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

		[DataMember]
		public List<LibraryState> States { get; set; }

		[DataMember]
		public List<LibraryDevicePresenter> Presenters { get; set; }

		[XmlIgnore]
		public string PresentationName
		{
			get
			{
				string result = null;
				if (Driver != null)
				{
					result = Driver.ShortName;
				}
				if (!string.IsNullOrEmpty(AlternativeName))
					result += " (" + AlternativeName + ")";
				return result;
			}
		}
	}
}