using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace SKDModule.Events
{
    public class ShowSKDZoneEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}