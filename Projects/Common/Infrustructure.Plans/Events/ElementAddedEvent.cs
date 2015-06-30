using System.Collections.Generic;
using Infrustructure.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class ElementAddedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}