using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		protected override void InitializeBrushes(ElementBase element, System.Windows.Rect rect)
		{
			base.InitializeBrushes(element, rect);
			SolidColorBrush.Color = Colors.Black;
			SolidColorBrush.Freeze();
			ImageBrush.ImageSource = null;
			ImageBrush.Freeze();
		}
		protected override void InitializePen(ElementBase element, System.Windows.Rect rect)
		{
			base.InitializePen(element, rect);
			Pen.Thickness = 0;
			Pen.Brush.Freeze();
			Pen.Freeze();
		}
	}
}