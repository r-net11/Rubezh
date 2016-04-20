﻿using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;

namespace Infrastructure.Plans.Events
{
	public class ElementAddedEvent : CompositePresentationEvent<List<ElementBase>>
	{
	}
}