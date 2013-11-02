using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace GKProcessor.Events
{
	public class NewXJournalEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}