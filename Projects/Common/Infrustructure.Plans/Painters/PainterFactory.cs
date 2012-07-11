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
		private static Dictionary<Primitive, IPainter> _painters = new Dictionary<Primitive, IPainter>()
		{
			{Primitive.Ellipse, new ElipsePainter()},
			{Primitive.Polygon, new PolygonPainter()},
			{Primitive.PolygonZone, new PolygonZonePainter()},
			{Primitive.Polyline, new PolylinePainter()},
			{Primitive.Rectangle, new RectanglePainter()},
			{Primitive.RectangleZone, new RectangleZonePainter()},
			{Primitive.SubPlan, new SubPlanPainter()},
			{Primitive.TextBlock, new TextBlockPainter()},
		};
		public static IPainter Create(ElementBase element)
		{
			Type type = element.GetType();
			if (element is IPrimitive)
				return _painters[((IPrimitive)element).Primitive];
			var args = new PainterFactoryEventArgs(element);
			EventService.EventAggregator.GetEvent<PainterFactoryEvent>().Publish(args);
			return args.Painter ?? new DefaultPainter();
		}
	}
}
