using FiresecAPI;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class MulticlientStateChangedEvent : CompositePresentationEvent<StateType>
	{
	}
}