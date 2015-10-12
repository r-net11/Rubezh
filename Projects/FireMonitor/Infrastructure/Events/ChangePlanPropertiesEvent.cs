using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.AutomationCallback;

namespace Infrastructure.Events
{
	public class ChangePlanPropertiesEvent : CompositePresentationEvent<List<PlanCallbackData>>
	{
	}
}
