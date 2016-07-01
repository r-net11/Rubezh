using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using System;
using System.Collections.Generic;

namespace Infrustructure.Plans.Painters
{
	public static class PainterFactory
	{
		private static readonly Dictionary<Primitive, Type> Painters = new Dictionary<Primitive, Type>
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

		public static IPainter Create(CommonDesignerCanvas designerCanvas, ElementBase element)
		{
			var primitive = element as IPrimitive;

			if (primitive != null && primitive.Primitive != Primitive.NotPrimitive)
				return (IPainter)Activator.CreateInstance(Painters[primitive.Primitive], designerCanvas, element);

			var args = new PainterFactoryEventArgs(designerCanvas, element);
			EventService.EventAggregator.GetEvent<PainterFactoryEvent>().Publish(args);

			return args.Painter ?? new DefaultPainter(designerCanvas, element);
		}
	}
}