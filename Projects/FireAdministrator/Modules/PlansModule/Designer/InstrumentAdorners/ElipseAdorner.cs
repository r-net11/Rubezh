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
using FiresecAPI.Models;

namespace PlansModule.InstrumentAdorners
{
	public class ElipseAdorner : RectangleAdorner
	{
		public ElipseAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Ellipse();
		}
		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			return new ElementEllipse();
		}
	}
}