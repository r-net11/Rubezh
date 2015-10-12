using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKAlarmsEvent : CompositePresentationEvent<GKAlarmType?>
	{
	}
}