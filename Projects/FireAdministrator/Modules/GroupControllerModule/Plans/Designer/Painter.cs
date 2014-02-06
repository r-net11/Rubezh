using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Painters;

namespace GKModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementXDevice _elementXDevice;
		private ImageTextTooltipViewModel _toolTip;
		public Painter(ElementXDevice elementXDevice)
			: base(elementXDevice)
		{
			_elementXDevice = elementXDevice;
			_toolTip = new ImageTextTooltipViewModel();
		}

		protected override Brush GetBrush()
		{
			var xdevice = Helper.GetXDevice(_elementXDevice);
			_toolTip.ImageSource = xdevice == null ? null : xdevice.Driver.ImageSource;
			return PictureCacheSource.XDevicePicture.GetXBrush(xdevice);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}