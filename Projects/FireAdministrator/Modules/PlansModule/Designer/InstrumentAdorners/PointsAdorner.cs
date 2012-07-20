using System.Windows.Input;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;

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