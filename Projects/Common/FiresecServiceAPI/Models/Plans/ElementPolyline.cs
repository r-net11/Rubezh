using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolyline : ElementBasePolyline, IElementZIndex
	{
		public ElementPolyline()
		{
			BackgroundColor = Colors.Black;
			BorderThickness = 1;

			Points = new PointCollection();
			Points.Add(new Point(0, 0));
			Points.Add(new Point(100, 100));
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override FrameworkElement Draw()
		{
			var polyline = new Polyline()
			{
				Points = new PointCollection(Points),
				Stroke = new SolidColorBrush(BackgroundColor),
				Fill = new SolidColorBrush(Colors.Transparent),
				StrokeThickness = BorderThickness
			};

			return polyline;
		}

		public override ElementBase Clone()
		{
			ElementPolyline elementLine = new ElementPolyline()
			{
				BackgroundColor = BackgroundColor,
				BorderThickness = BorderThickness,
				Points = Points.Clone()
			};
			Copy(elementLine);
			return elementLine;
		}
	}
}