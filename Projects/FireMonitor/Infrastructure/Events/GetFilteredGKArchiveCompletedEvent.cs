using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;
using System;

namespace Infrastructure.Events
{
	public class GetFilteredGKArchiveCompletedEvent : CompositePresentationEvent<ArchiveResult>
	{
	}

	public class ArchiveResult
	{
		public IEnumerable<JournalItem> JournalItems { get; set; }
		public Guid ArchivePortionUID { get; set; }
	}
}