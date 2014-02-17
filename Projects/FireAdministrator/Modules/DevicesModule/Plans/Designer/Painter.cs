using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;

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