using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class FindElementEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}
