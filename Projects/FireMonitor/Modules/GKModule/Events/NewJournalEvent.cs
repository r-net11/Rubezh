using System.Collections.Generic;
using Common.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class NewJournalEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}