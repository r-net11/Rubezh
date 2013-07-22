using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowParameterTemplatesEvent : CompositePresentationEvent<Guid>
	{
	}
}