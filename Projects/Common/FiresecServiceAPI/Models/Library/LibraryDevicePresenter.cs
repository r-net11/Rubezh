using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class LibraryDevicePresenter
	{
		public LibraryDevicePresenter()
		{
			States = new List<LibraryState>()
			{
				new LibraryState()
				{
					StateType = StateType.No
				}
			};
		}

		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public List<LibraryState> States { get; set; }
	}
}
