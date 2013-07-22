using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Painters;

namespace GKModule.Plans.Designer
{
	public class Painter : PointPainter
	{
		private ElementXDevice _elementXDevice;
		public Painter(ElementXDevice elementXDevice)
			: base(elementXDevice)
		{
			_elementXDevice = elementXDevice;
		}

		protected override Brush GetBrush()
		{
			var xdevice = Helper.GetXDevice(_elementXDevice);
			return DevicePictureCache.GetXBrush(xdevice);
		}
	}
}