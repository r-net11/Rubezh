using System;
using Microsoft.Practices.Prism.Events;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Events
{
	public class ElementSelectedEvent : CompositePresentationEvent<ElementBase>
	{
	}
}
