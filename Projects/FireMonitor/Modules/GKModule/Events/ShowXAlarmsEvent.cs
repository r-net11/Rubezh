using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowXAlarmsEvent : CompositePresentationEvent<GKAlarmType?>
	{
	}
}