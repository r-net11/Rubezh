using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Journal;

namespace Infrastructure.Events
{
	public class UpdateJournalItemsEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}