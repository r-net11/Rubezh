using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Painters
{
	public static class PainterFactory
	{
		public static IPainter Create(ElementBase element)
		{
			Type type = element.GetType();
			if (element is IPrimitive)
				switch (((IPrimitive)element).Primitive)
				{
					case Primitive.Ellipse:
						return new ElipsePainter();
					case Primitive.Polygon:
						return new PolygonPainter();
					case Primitive.PolygonZone:
						return new PolygonZonePainter();
					case Primitive.Polyline:
						return new PolylinePainter();
					case Primitive.Rectangle:
						return new RectanglePainter();
					case Primitive.RectangleZone:
						return new RectangleZonePainter();
					case Primitive.SubPlan:
						return new SubPlanPainter();
					case Primitive.TextBlock:
						return new TextBlockPainter();
				}
			var args = new PainterFactoryEventArgs(element);
			EventService.EventAggregator.GetEvent<PainterFactoryEvent>().Publish(args);
			return args.Painter ?? new DefaultPainter();
		}
	}
}
