using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace Infrastructure.Events
{
	public class ShowXAlarmsEvent : CompositePresentationEvent<XStateType?>
	{
	}
}