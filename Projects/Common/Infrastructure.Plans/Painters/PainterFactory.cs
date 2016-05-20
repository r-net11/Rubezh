using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Events;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;

namespace Infrastructure.Plans.Painters
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
			{Primitive.TextBox, typeof(TextBoxPainter)},
			{Primitive.Procedure, typeof(ProcedurePainter)}
		};
		public static IPainter Create(CommonDesignerCanvas designerCanvas, ElementBase element)
		{
			Type type = element.GetType();
			var primitive = element as IPrimitive;
			if (primitive != null && primitive.Primitive != Primitive.NotPrimitive)
				return (IPainter)Activator.CreateInstance(_painters[primitive.Primitive], designerCanvas, element);
			var args = new PainterFactoryEventArgs(designerCanvas, element);
			EventService.EventAggregator.GetEvent<PainterFactoryEvent>().Publish(args);
			return args.Painter ?? new DefaultPainter(designerCanvas, element);
		}
	}
}