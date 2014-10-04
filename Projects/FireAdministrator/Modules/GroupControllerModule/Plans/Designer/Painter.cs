using System.Windows.Media;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

namespace GKModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementGKDevice _elementGKDevice;
		private ImageTextTooltipViewModel _toolTip;
		public Painter(CommonDesignerCanvas designerCanvas, ElementGKDevice elementGKDevice)
			: base(designerCanvas, elementGKDevice)
		{
			_elementGKDevice = elementGKDevice;
			_toolTip = new ImageTextTooltipViewModel();
		}

		protected override Brush GetBrush()
		{
			var device = GKPlanExtension.Instance.GetItem<GKDevice>(_elementGKDevice);
			_toolTip.ImageSource = device == null ? null : device.Driver.ImageSource;
			return PictureCacheSource.GKDevicePicture.GetBrush(device);
		}

		public override object GetToolTip(string title)
		{
			_toolTip.Title = title;
			return _toolTip;
		}
	}
}