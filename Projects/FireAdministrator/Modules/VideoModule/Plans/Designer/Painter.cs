using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;

namespace VideoModule.Plans.Designer
{
	internal class Painter : PointPainter
	{
		private ElementCamera _elementCamera;
		public Painter(CommonDesignerCanvas designerCanvas, ElementCamera elementCamera)
			: base(designerCanvas, elementCamera)
		{
			_elementCamera = elementCamera;
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.CameraPicture.GetDefaultBrush();
		}
	}
}