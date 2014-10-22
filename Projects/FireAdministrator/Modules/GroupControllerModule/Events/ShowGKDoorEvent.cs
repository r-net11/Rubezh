using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace GKModule.Events
{
    public class ShowGKDoorEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}