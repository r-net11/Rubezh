using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;
using System;

namespace GKModule.Events
{
	public class ShowGKDirectionEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}