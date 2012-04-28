using Microsoft.Practices.Prism.Events;
using FiresecAPI.Models;

namespace AlarmModule.Events
{
	public class ShowAlarmsEvent : CompositePresentationEvent<AlarmType?>
	{
	}
}