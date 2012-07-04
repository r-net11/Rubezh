using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IElementZIndex
	{
		public ElementPolygon()
		{

		}

		public override FrameworkElement Draw()
		{
			var polygon = new Polygon()
			{
				Points = new PointCollection(Points),
				Fill = new SolidColorBrush(BackgroundColor),
				Stroke = new SolidColorBrush(BorderColor),
				StrokeThickness = BorderThickness
			};

			if (BackgroundPixels != null)
			{
				polygon.Fill = PlanElementsHelper.CreateBrush(BackgroundPixels);
			}

			return polygon;
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementPolygon();
			Copy(elementBase);
			return elementBase;
		}
	}
}