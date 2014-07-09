using System.Collections.Generic;
using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;
using System;
using FiresecAPI.Journal;

namespace Infrastructure.Events
{
	public class GetFilteredSKDArchiveCompletedEvent : CompositePresentationEvent<SKDArchiveResult>
	{
	}

	public class SKDArchiveResult
	{
		public IEnumerable<JournalItem> JournalItems { get; set; }
		public Guid ArchivePortionUID { get; set; }
	}
}