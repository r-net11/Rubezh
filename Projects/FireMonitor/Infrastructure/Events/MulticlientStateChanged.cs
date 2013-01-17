using FiresecAPI;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class MulticlientStateChanged : CompositePresentationEvent<StateType>
	{
	}
}