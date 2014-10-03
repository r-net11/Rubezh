using System.Collections.Generic;
using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class NewXJournalEvent : CompositePresentationEvent<List<GKJournalItem>>
	{
	}
}