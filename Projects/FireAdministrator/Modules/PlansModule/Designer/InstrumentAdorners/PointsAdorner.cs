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
	public class PointsAdorner : InstrumentAdorner
	{
		public PointsAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override void Show()
		{
			AdornerCanvas.Cursor = Cursors.No;
		}
	}
}