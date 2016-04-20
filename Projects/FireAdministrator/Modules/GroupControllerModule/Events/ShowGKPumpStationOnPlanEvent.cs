using System;
using Infrastructure.Common.Windows.Navigation;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKPumpStationOnPlanEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}