using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class GetFilteredGKArchiveCompletedEvent : CompositePresentationEvent<ArchiveResult>
	{
	}

	public class ArchiveResult
	{
		public IEnumerable<XJournalItem> JournalItems { get; set; }
		public Guid ArchivePortionUID { get; set; }
	}
}