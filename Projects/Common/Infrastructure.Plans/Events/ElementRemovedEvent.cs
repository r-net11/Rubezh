using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;

namespace Infrastructure.Plans.Events
{
	public class ElementRemovedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}