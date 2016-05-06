using System;
using System.Collections.Generic;
using StrazhAPI.Journal;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class GetFilteredArchiveCompletedEvent : CompositePresentationEvent<ArchiveResult>
	{
	}

	public class ArchiveResult
	{
		public IEnumerable<JournalItem> JournalItems { get; set; }
		public Guid ArchivePortionUID { get; set; }
	}
}