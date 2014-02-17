using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Microsoft.Practices.Prism.Events;

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