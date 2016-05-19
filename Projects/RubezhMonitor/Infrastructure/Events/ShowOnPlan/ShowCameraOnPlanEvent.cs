using RubezhAPI.Models;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowCameraOnPlanEvent : CompositePresentationEvent<Camera>
	{
	}
}