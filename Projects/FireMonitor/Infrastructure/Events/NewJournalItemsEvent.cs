using System.Collections.Generic;
using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class NewJournalItemsEvent : CompositePresentationEvent<List<FiresecAPI.SKD.JournalItem>>
	{
	}
}