using StrazhAPI.Plans.Elements;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace Infrustructure.Plans.Events
{
	public class ElementRemovedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}