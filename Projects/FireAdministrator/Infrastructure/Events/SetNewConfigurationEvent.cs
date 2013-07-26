using Microsoft.Practices.Prism.Events;
using System.ComponentModel;

namespace Infrastructure.Events
{
	public class SetNewConfigurationEvent : CompositePresentationEvent<CancelEventArgs>
    {
    }
}