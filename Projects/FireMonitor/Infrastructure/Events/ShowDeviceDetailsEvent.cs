using Infrastructure.Models;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowDeviceDetailsEvent : CompositePresentationEvent<ElementDeviceReference>
	{
	}
}