using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;
using System;

namespace GKModule.Plans.Designer
{
	public class GKDoorPainter : PointPainter
	{
		private ElementGKDoor _elementGKDoor;
		private ImageTextTooltipViewModel _toolTip;
		public GKDoorPainter(CommonDesignerCanvas designerCanvas, ElementGKDoor elementGKDoor)
			: base(designerCanvas, elementGKDoor)
		{
			_elementGKDoor = elementGKDoor;
			_toolTip = new ImageTextTooltipViewModel();
			_toolTip.ImageSource = "/Controls;component/Images/Door.png";
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.DoorPicture.GetDefaultBrush(_elementGKDoor.DoorUID != Guid.Empty);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}