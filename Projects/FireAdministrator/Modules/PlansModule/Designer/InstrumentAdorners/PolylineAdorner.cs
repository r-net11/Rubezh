using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;

namespace PlansModule.InstrumentAdorners
{
	public class PolylineAdorner : PolygonAdorner
	{
		public PolylineAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Polyline();
		}
		protected override PointCollection Points
		{
			get { return ((Polyline)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			return new ElementPolyline();
		}
	}
}