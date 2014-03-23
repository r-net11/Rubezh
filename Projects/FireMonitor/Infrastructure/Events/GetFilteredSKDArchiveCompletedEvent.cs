using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;
using FiresecAPI;

namespace Infrastructure.Events
{
	public class GetFilteredSKDArchiveCompletedEvent : CompositePresentationEvent<IEnumerable<SKDJournalItem>>
	{
	}
}