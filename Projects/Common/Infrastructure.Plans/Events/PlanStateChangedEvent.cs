using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class PlanStateChangedEvent : CompositePresentationEvent<Guid>
	{
	}
}