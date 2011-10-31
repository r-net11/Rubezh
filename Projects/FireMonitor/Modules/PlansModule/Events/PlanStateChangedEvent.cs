using Microsoft.Practices.Prism.Events;
using System;

namespace PlansModule.Events
{
    public class PlanStateChangedEvent : CompositePresentationEvent<Guid>
    {
    }
}