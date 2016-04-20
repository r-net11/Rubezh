using System;
using Infrastructure.Common.Windows.Navigation;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowPlanElementEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}
