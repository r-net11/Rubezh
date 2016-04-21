using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class FindElementEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}