using System.Collections.Generic;
using FiresecAPI;
using Microsoft.Practices.Prism.Events;

namespace SKDModule.Events
{
	public class NewSKDJournalEvent : CompositePresentationEvent<List<SKDJournalItem>>
	{
	}
}