using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

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