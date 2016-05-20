using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.Painters
{
	public class PolygonZonePainter : PolygonPainter
	{
		ImageTextTooltipViewModel _toolTip;
		public PolygonZonePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
			_toolTip = new ImageTextTooltipViewModel();
			if (element is ElementPolygonGKZone)
				_toolTip.ImageSource = "/Controls;component/Images/Zone.png";
			else if (element is ElementPolygonGKSKDZone)
				_toolTip.ImageSource = "/Controls;component/Images/SKDZone.png";
			else if (element is ElementPolygonGKGuardZone)
				_toolTip.ImageSource = "/Controls;component/Images/GuardZone.png";
			else if (element is ElementPolygonGKDirection)
				_toolTip.ImageSource = "/Controls;component/Images/BDirection.png";
			else if (element is ElementPolygonGKMPT)
				_toolTip.ImageSource = "/Controls;component/Images/BMPT.png";
			else if (element is ElementPolygonGKPumpStation)
				_toolTip.ImageSource = "/Controls;component/Images/BPumpStation.png";
			else if (element is ElementPolygonGKDelay)
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