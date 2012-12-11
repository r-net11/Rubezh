﻿using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : ShapePainter<Rectangle>
	{
		public override Visual Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Opacity = 0.5;
			return shape;
		}
	}
}
