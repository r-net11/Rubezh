using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Events
{
	public class PainterFactoryEvent : CompositePresentationEvent<PainterFactoryEventArgs>
	{
	}

	public class PainterFactoryEventArgs
	{
		public PainterFactoryEventArgs(ElementBase element)
		{
			Element = element;
		}

		public ElementBase Element { get; private set; }
		public IPainter Painter { get; set; }
	}
}
