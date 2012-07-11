using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrustructure.Plans.Elements;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : ShapePainter<Rectangle>
	{
		#region IPainter Members

		public override FrameworkElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Fill = Brushes.Black;
			return shape;
		}

		#endregion
	}
}