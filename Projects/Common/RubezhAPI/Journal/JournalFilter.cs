using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace RubezhAPI.Journal
{
	[DataContract]
	public class JournalFilter
	{
		public static int DefaultLastItemsCount { get { return 100; } }
		public JournalFilter()
		{
			UID = Guid.NewGuid();
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
			LastItemsCount = DefaultLastItemsCount;
			PageSize = 100;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? ItemUID { get; set; }

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

		[DataMember]
		public bool IsSortAsc { get; set; }

		[DataMember]
		public int LastItemsCount { get; set; }
	}
}