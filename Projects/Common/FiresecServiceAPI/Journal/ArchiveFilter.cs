using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Journal
{
	[DataContract]
	public class ArchiveFilter
	{
		public ArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			UseDeviceDateTime = false;
			JournalSubsystemTypes = new List<JournalSubsystemType>();
			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalEventDescriptionTypes = new List<JournalEventDescriptionType>();
			JournalObjectTypes = new List<JournalObjectType>();
			ObjectUIDs = new List<Guid>();
			EmployeeUIDs = new List<Guid>();
			Users = new List<string>();
		}

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<JournalSubsystemType> JournalSubsystemTypes { get; set; }

		[DataMember]
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }

		[DataMember]
		public List<JournalEventDescriptionType> JournalEventDescriptionTypes { get; set; }

		[DataMember]
		public List<JournalObjectType> JournalObjectTypes { get; set; }

		[DataMember]
		public List<Guid> ObjectUIDs { get; set; }

		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }

		[DataMember]
		public List<string> Users { get; set; }

		[DataMember]
		public int PageSize { get; set; }
	}
}