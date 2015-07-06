using Microsoft.Practices.Prism.Events;
using System;

namespace Infrustructure.Plans.Events
{
	public class PlanStateChangedEvent : CompositePresentationEvent<Guid>
	{
	}
}