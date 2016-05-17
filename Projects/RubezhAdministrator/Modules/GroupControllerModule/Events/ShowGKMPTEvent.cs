using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;
using System;

namespace GKModule.Events
{
	public class ShowGKMPTEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}