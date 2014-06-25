using System.Collections.Generic;
using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class GetFilteredSKDArchiveCompletedEvent : CompositePresentationEvent<IEnumerable<JournalItem>>
	{
	}
}