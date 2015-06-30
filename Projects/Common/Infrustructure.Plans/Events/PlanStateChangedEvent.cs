using System;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class PlanStateChangedEvent : CompositePresentationEvent<Guid>
	{
	}
}