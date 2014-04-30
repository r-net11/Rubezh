using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowVideoEvent : CompositePresentationEvent<Guid>
	{
	}
}