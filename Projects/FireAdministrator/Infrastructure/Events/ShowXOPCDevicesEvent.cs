using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowXOPCDevicesEvent : CompositePresentationEvent<Guid>
	{
	}
}