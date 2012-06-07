using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;

namespace AlarmModule.Events
{
	public class ShowAlarmsEvent : CompositePresentationEvent<AlarmType?>
	{
	}
}