using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

namespace SKDModule.Plans.Designer
{
	public class DoorPainter : PointPainter
	{
		private ElementDoor _elementDoor;
		private ImageTextTooltipViewModel _toolTip;
		public DoorPainter(CommonDesignerCanvas designerCanvas, ElementDoor elementDoor)
			: base(designerCanvas, elementDoor)
		{
			_elementDoor = elementDoor;
			_toolTip = new ImageTextTooltipViewModel();
			_toolTip.ImageSource = "/Controls;component/Images/Door.png";
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.DoorPicture.GetDefaultBrush();
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}