using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Painters;

namespace DevicesModule.Plans.Designer
{
	internal class Painter : PointPainter
	{
		private ElementDevice _elementDevice;
		public Painter(CommonDesignerCanvas designerCanvas, ElementDevice elementDevice)
			: base(designerCanvas, elementDevice)
		{
			_elementDevice = elementDevice;
		}

		protected override Brush GetBrush()
		{
			var device = Helper.GetDevice(_elementDevice);
			return PictureCacheSource.DevicePicture.GetBrush(device);
		}
	}
}