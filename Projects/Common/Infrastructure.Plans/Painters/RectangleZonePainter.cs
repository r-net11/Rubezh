using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.Painters
{
	public class RectangleZonePainter : RectanglePainter
	{
		ImageTextTooltipViewModel _toolTip;
		public RectangleZonePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
			_toolTip = new ImageTextTooltipViewModel();
			if (element is ElementRectangleGKZone)
				_toolTip.ImageSource = "/Controls;component/Images/Zone.png";
			else if (element is ElementRectangleGKSKDZone)
				_toolTip.ImageSource = "/Controls;component/Images/SKDZone.png";
			else if (element is ElementRectangleGKGuardZone)
				_toolTip.ImageSource = "/Controls;component/Images/GuardZone.png";
			else if (element is ElementRectangleGKDirection)
				_toolTip.ImageSource = "/Controls;component/Images/BDirection.png";
			else if (element is ElementRectangleGKMPT)
				_toolTip.ImageSource = "/Controls;component/Images/BMPT.png";
			else if (element is ElementRectangleGKPumpStation)
				_toolTip.ImageSource = "/Controls;component/Images/BPumpStation.png";
			else if (element is ElementRectangleGKDelay)
				_toolTip.ImageSource = "/Controls;component/Images/Delay.png";

		}

		protected override Brush GetBrush()
		{
			return DesignerCanvas.PainterCache.GetTransparentBrush(Element);
		}
		protected override Pen GetPen()
		{
			return DesignerCanvas.PainterCache.ZonePen;
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}