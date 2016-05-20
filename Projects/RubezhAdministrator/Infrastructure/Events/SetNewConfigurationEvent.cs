using System.ComponentModel;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class SetNewConfigurationEvent : CompositePresentationEvent<CancelEventArgs>
	{
	}
}