using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class SelectPlanEvent : CompositePresentationEvent<Guid>
	{
	}
}