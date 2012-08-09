using Microsoft.Practices.Prism.Events;
using Common.GK;
using System.Collections.Generic;

namespace GKModule.Events
{
	public class NewJournalEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}