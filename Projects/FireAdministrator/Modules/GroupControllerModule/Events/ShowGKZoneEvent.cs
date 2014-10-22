using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace GKModule.Events
{
    public class ShowGKZoneEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
    {
    }
}