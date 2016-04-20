using System.Windows;
using System.Windows.Media;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartSize
	{
		public GridUnitType WidthType { get; set; }
		public GridUnitType HeightType { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }
		public bool IsWidthFixed { get; set; }
		public bool IsHeightFixed { get; set; }
		public int Margin { get; set; }
		public Color BackgroundColor { get; set; }
		public Color BorderColor { get; set; }
		public int BorderThickness { get; set; }
		public Size PreferedSize { get; set; }
	}
}