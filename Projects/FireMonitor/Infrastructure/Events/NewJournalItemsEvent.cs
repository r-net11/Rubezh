using System.Collections.Generic;
using StrazhAPI.Models;
using Microsoft.Practices.Prism.Events;
using StrazhAPI.Journal;

namespace Infrastructure.Events
{
	public class NewJournalItemsEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}