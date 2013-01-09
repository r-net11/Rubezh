using System;
using Microsoft.Practices.Prism.Events;
using FiresecAPI;

namespace Infrastructure.Events
{
	public class MulticlientStateChanged : CompositePresentationEvent<StateType>
	{
	}
}