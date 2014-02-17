using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;

namespace SKDModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementSKDDevice _elementSKDDevice;
		private ImageTextTooltipViewModel _toolTip;
		public Painter(CommonDesignerCanvas designerCanvas, ElementSKDDevice elementSKDDevice)
			: base(designerCanvas, elementSKDDevice)
		{
			_elementSKDDevice = elementSKDDevice;
			_toolTip = new ImageTextTooltipViewModel();
		}

		protected override Brush GetBrush()
		{
			var device = Helper.GetSKDDevice(_elementSKDDevice);
			_toolTip.ImageSource = device == null ? null : device.Driver.ImageSource;
			return PictureCacheSource.SKDDevicePicture.GetBrush(device);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}