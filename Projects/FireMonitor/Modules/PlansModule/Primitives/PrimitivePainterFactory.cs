using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;
using PlansModule.ViewModels;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;

namespace PlansModule.Primitives
{
	/// <summary>
	/// Creates Painters for Primitives.
	/// </summary>
	public static class PrimitivePainterFactory
	{
		/// <summary>
		/// Creates Painter for specified Primitive.
		/// </summary>
		/// <param name="canvas">Canvase to create Painter for.</param>
		/// <param name="element">Element to create Painter for.</param>
		/// <returns>Created Painter Instance.</returns>
		public static IPainter CreatePainter(CommonDesignerCanvas canvas, ElementBase element)
		{
			if (canvas == null)
				throw new ArgumentNullException("canvas");
			if (element == null)
				throw new ArgumentNullException("element");
			IPrimitive primitive = element as IPrimitive;
			if (primitive == null)
				throw new ArgumentException("Element must be a Primitive", "element");

			Func<CommonDesignerCanvas, ElementBase, IPainter> factoryMethod = null;
			if (factoryMethods.TryGetValue(primitive.Primitive, out factoryMethod))
			{
				// The following Object contains a Tooltip Flag to be copied from Source Element to determine Visibility:
				ToolTipFlagContainer tooltipFlag = new ToolTipFlagContainer();
				ElementBase.Copy(element, tooltipFlag);

				IPainter wrappedPainter = factoryMethod(canvas, element);
				PrimitiveToolTipViewModel tooltip = new PrimitiveToolTipViewModel()
				{
					Name = element.PresentationName,
				};
				return new PrimitivePainter(wrappedPainter)
				{
					ToolTip = tooltipFlag.ShowTooltip ? tooltip : null,
				};
			}
			else
				throw new InvalidOperationException("Unknown Primitive Type");
		}

		private static IDictionary<Primitive, Func<CommonDesignerCanvas, ElementBase, IPainter>> factoryMethods = new Dictionary<Primitive, Func<CommonDesignerCanvas, ElementBase, IPainter>>()
		{
			{ Primitive.Rectangle, (canvas, element) => new RectanglePainter(canvas, element) },
			{ Primitive.Ellipse, (canvas, element) => new ElipsePainter(canvas, element) },
			{ Primitive.TextBlock, (canvas, element) => new TextBlockPainter(canvas, element) },
			{ Primitive.TextBox, (canvas, element) => new TextBoxPainter(canvas, element) },
			{ Primitive.Polygon, (canvas, element) => new PolygonPainter(canvas, element) },
			{ Primitive.Polyline, (canvas, element) => new PolylinePainter(canvas, element) },
		};

		/// <summary>
		/// The following Class contains a Tooltip Flag to be copied from Source Element to determine Visibility:
		/// </summary>
		private class ToolTipFlagContainer
		{
			public bool ShowTooltip { get; set; }
		}
	}
}