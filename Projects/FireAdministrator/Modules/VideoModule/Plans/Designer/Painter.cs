using System.Windows.Media;
using DeviceControls;
using StrazhAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;
using System;

namespace VideoModule.Plans.Designer
{
	internal class Painter : PointPainter
	{
		private ElementCamera _elementCamera;
		private ImageTextTooltipViewModel _toolTip;
		public Painter(CommonDesignerCanvas designerCanvas, ElementCamera elementCamera)
			: base(designerCanvas, elementCamera)
		{
			_elementCamera = elementCamera;
			_toolTip = new ImageTextTooltipViewModel();
			_toolTip.ImageSource = "/Controls;component/Images/BVideo.png";
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.CameraPicture.GetDefaultBrush(_elementCamera.CameraUID != Guid.Empty);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}