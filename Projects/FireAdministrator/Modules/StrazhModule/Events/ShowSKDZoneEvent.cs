using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace StrazhModule.Events
{
	public class ShowSKDZoneEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}