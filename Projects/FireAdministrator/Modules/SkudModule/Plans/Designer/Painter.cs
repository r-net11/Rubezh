using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Painters;

namespace SKDModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementSKDDevice _elementSKDDevice;
		private ImageTextTooltipViewModel _toolTip;
		public Painter(ElementSKDDevice elementSKDDevice)
			: base(elementSKDDevice)
		{
			_elementSKDDevice = elementSKDDevice;
			_toolTip = new ImageTextTooltipViewModel();
		}

		protected override Brush GetBrush()
		{
			var device = Helper.GetSKDDevice(_elementSKDDevice);
			_toolTip.ImageSource = device == null ? null : device.Driver.ImageSource;
			return PictureCacheSource.SKDDevicePicture.GetSKDBrush(device);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}