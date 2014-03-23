using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;
using FiresecAPI.SKD.PassCardLibrary;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

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
					_brush = new ImageBrush(bitmapImage);
					_brush.Freeze();
				}
			else
				_brush = null;

		}
		protected override Brush GetBrush()
		{
			return _brush;
		}
	}
}