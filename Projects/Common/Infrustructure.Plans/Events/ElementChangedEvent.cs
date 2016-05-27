using StrazhAPI.Plans.Elements;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace Infrustructure.Plans.Events
{
	public class ElementChangedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}