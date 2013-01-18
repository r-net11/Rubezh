using System;
using System.Collections.Generic;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace Infrustructure.Plans.Painters
{
	public static class PainterFactory
	{
		private static Dictionary<Primitive, Type> _painters = new Dictionary<Primitive, Type>()
		{
			{Primitive.Ellipse, typeof(ElipsePainter)},
			{Primitive.Polygon, typeof(PolygonPainter)},
			{Primitive.PolygonZone, typeof(PolygonZonePainter)},
			{Primitive.Polyline, typeof(PolylinePainter)},
			{Primitive.Rectangle, typeof(RectanglePainter)},
			{Primitive.RectangleZone, typeof(RectangleZonePainter)},
			{Primitive.SubPlan, typeof(SubPlanPainter)},
			{Primitive.TextBlock, typeof(TextBlockPainter)},
		};
		public static IPainter Create(ElementBase element)
		{
			Type type = element.GetType();
			if (element is IPrimitive)
				return (IPainter)Activator.CreateInstance(_painters[((IPrimitive)element).Primitive]);
			var args = new PainterFactoryEventArgs(element);
			EventService.EventAggregator.GetEvent<PainterFactoryEvent>().Publish(args);
			return args.Painter ?? new DefaultPainter();
		}
	}
}
