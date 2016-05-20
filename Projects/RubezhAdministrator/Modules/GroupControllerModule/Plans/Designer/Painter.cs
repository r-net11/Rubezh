using DeviceControls;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System.Windows.Media;

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
			var device = GKPlanExtension.Instance.GetItem<GKDevice>(_elementGKDevice.ItemUID);
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