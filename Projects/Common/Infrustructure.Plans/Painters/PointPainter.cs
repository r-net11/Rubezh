using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class PointPainter : RectanglePainter
	{
		public PointPainter(ElementBase element)
			: base(element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			return PainterCache.PointGeometry;
		}
		protected override Pen GetPen()
		{
			return null;
		}
		public override void Transform()
		{
		}
	}
}
