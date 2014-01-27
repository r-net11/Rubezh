using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using FiresecAPI;

namespace Infrastructure.Events
{
	public class NewSKDJournalEvent : CompositePresentationEvent<List<SKDJournalItem>>
	{
	}
}