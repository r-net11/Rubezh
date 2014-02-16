using Microsoft.Practices.Prism.Events;
using XFiresecAPI;
using FiresecAPI.Models;

namespace Infrastructure.Events
{
	public class ShowCameraOnPlanEvent : CompositePresentationEvent<Camera>
	{
	}
}