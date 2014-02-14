using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Painters;

namespace VideoModule.Plans.Designer
{
	internal class Painter : PointPainter
	{
		private ElementCamera _elementCamera;
		public Painter(ElementCamera elementCamera)
			: base(elementCamera)
		{
			_elementCamera = elementCamera;
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.CameraBrush;
		}
	}
}