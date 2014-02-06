using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Devices;

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
			var libraryState = new LibraryState()
			{
				StateType = StateType.No,
			};
			States.Add(libraryState);
			Presenters = new List<LibraryDevicePresenter>();
		}

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