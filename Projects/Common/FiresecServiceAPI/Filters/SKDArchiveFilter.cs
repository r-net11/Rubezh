using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace FiresecAPI.Journal
{
	[DataContract]
	public class SKDArchiveFilter
	{
		public SKDArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			JournalObjectTypes = new List<JournalObjectType>();
			StateClasses = new List<XStateClass>();
			Descriptions = new List<string>();
			UseDeviceDateTime = false;

			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalSubsystemTypes = new List<JournalSubsystemType>();
			ObjectUIDs = new List<Guid>();
		}

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }

		[DataMember]
		public List<string> Descriptions { get; set; }

		[DataMember]
		public List<JournalSubsystemType> JournalSubsystemTypes { get; set; }

		[DataMember]
		public List<JournalObjectType> JournalObjectTypes { get; set; }

		[DataMember]
		public List<Guid> ObjectUIDs { get; set; }

		[DataMember]
		public int PageSize { get; set; }


		[DataMember]
		public List<XStateClass> StateClasses { get; set; }
	}
}