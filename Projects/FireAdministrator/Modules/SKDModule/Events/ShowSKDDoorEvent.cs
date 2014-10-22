using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace SKDModule.Events
{
    public class ShowSKDDoorEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}