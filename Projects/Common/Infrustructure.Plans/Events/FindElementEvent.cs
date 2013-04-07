using System;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace Infrustructure.Plans.Events
{
	public class FindElementEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}
