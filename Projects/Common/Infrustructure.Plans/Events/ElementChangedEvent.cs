﻿using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;

namespace Infrustructure.Plans.Events
{
	public class ElementChangedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}