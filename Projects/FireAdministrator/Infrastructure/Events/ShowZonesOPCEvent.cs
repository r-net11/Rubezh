using System;
using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowZonesOPCEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}
