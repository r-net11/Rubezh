using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.SKD;

namespace StrazhAPI.Journal
{
	[DataContract]
	public class JournalFilter : FilterBase
	{
		public JournalFilter()
		{
			UID = Guid.NewGuid();
			JournalSubsystemTypes = new List<JournalSubsystemType>();
			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalEventDescriptionTypes = new List<JournalEventDescriptionType>();
			JournalObjectTypes = new List<JournalObjectType>();
			ObjectUIDs = new List<Guid>();
			LastItemsCount = 100;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int LastItemsCount { get; set; }

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
	}
}