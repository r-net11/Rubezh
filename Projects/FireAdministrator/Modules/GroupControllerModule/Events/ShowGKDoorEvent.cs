using System;
using Infrastructure.Common.Windows.Navigation;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKDoorEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}