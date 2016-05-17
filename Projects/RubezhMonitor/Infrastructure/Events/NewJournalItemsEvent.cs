using System.Collections.Generic;
using RubezhAPI.Models;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Journal;

namespace Infrastructure.Events
{
	public class NewJournalItemsEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}