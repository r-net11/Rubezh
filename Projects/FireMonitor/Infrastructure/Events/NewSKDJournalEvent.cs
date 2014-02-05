using System.Collections.Generic;
using FiresecAPI;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class NewSKDJournalEvent : CompositePresentationEvent<List<SKDJournalItem>>
	{
	}
}