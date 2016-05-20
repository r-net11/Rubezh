using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.Painters
{
	public class ProcedurePainter : TextBlockPainter
	{
		ImageTextTooltipViewModel _toolTip;
		public ProcedurePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
			_toolTip = new ImageTextTooltipViewModel();
			_toolTip.ImageSource = "/Controls;component/Images/ProcedureYellow.png";
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}