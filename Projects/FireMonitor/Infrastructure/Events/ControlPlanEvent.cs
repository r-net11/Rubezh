using Microsoft.Practices.Prism.Events;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;

namespace Infrastructure.Events
{
	public class ControlPlanEvent : CompositePresentationEvent<ControlPlanEventArg>
	{
	}

	public class ControlPlanEventArg
	{
		public ControlElementType ControlElementType { get; set; }
		public PlanCallbackData PlanCallbackData { get; set; }
	}
}