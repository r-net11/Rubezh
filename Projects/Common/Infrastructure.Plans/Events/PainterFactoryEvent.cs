using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using Infrastructure.Plans.Painters;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class PainterFactoryEvent : CompositePresentationEvent<PainterFactoryEventArgs>
	{
	}

	public class PainterFactoryEventArgs
	{
		public PainterFactoryEventArgs(CommonDesignerCanvas designerCanvas, ElementBase element)
		{
			Element = element;
			DesignerCanvas = designerCanvas;
		}

		public ElementBase Element { get; private set; }
		public IPainter Painter { get; set; }
		public CommonDesignerCanvas DesignerCanvas { get; set; }
	}
}