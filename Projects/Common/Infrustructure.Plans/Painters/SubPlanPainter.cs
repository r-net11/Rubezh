using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrustructure.Plans.Elements;
using System.Windows.Shapes;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : ShapePainter<Rectangle>
	{
		public override FrameworkElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Opacity = 0.5;
			return shape;
		}
	}
}
