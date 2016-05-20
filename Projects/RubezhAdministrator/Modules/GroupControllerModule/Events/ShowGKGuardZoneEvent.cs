using System;
using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKGuardZoneEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}