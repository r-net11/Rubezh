﻿using Infrustructure.Plans.Elements;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace Infrustructure.Plans.Events
{
	public class ElementAddedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}