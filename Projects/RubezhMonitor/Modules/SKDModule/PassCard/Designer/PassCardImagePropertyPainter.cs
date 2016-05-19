using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RubezhAPI.SKD;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans;

namespace SKDModule.PassCard.Designer
{
	class PassCardImagePropertyPainter : RectanglePainter
	{
		private Brush _brush;
		public PassCardImagePropertyPainter(CommonDesignerCanvas designerCanvas, ElementPassCardImageProperty element, byte[] data)
			: base(designerCanvas, element)
		{
			if (data != null)
				using (var imageStream = new MemoryStream(data))
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.StreamSource = imageStream;
					bitmapImage.EndInit();
					_brush = new ImageBrush(bitmapImage)
					{
						Stretch = element.Stretch.ToWindowsStretch(),
					};
					_brush.Freeze();
				}
			else
				_brush = null;

		}
		protected override void InnerDraw(DrawingContext drawingContext)
		{
			base.InnerDraw(drawingContext);
			drawingContext.DrawGeometry(_brush, Pen, Geometry);
		}
	}
}