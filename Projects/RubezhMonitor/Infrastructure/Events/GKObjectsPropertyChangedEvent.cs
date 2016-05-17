using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class GKObjectsPropertyChangedEvent : CompositePresentationEvent<GKPropertyChangedCallback>
	{
	}
}