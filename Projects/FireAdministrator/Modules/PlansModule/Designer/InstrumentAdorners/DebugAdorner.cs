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

namespace PlansModule.InstrumentAdorners
{
	public class DebugAdorner : InstrumentAdorner
	{
		public DebugAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override void Show()
		{
			AdornerCanvas.Cursor = Cursors.Pen;
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			Hide();
		}
	}
}