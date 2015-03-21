using System;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Navigation;

namespace StrazhModule.Events
{
	public class ShowSKDDoorEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}