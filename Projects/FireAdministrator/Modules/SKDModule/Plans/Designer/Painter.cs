using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;
using SKDModule.Plans.ViewModels;

namespace SKDModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementSKDDevice _elementSKDDevice;
		private DeviceTooltipViewModel _toolTip;
		public Painter(CommonDesignerCanvas designerCanvas, ElementSKDDevice elementSKDDevice)
			: base(designerCanvas, elementSKDDevice)
		{
			_elementSKDDevice = elementSKDDevice;
			_toolTip = new DeviceTooltipViewModel();
		}

		protected override Brush GetBrush()
		{
			var device = SKDPlanExtension.Instance.GetItem<SKDDevice>(_elementSKDDevice);
			_toolTip.ImageSource = device == null ? null : device.Driver.ImageSource;
			if (device != null)
			{
				_toolTip.IsDeviceExists = true;
				_toolTip.ParentImageSource = device.Parent == null ? null : device.Parent.Driver.ImageSource;
				_toolTip.ParentTitle = device.Parent == null ? "Неизвестное устройство" : device.Parent.Name;
			}
			else
				_toolTip.IsDeviceExists = false;
			return PictureCacheSource.SKDDevicePicture.GetBrush(device);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}