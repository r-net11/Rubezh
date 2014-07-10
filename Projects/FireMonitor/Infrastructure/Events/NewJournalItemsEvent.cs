using System.Collections.Generic;
using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;
using FiresecAPI.Journal;

namespace Infrastructure.Events
{
	public class NewJournalItemsEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}