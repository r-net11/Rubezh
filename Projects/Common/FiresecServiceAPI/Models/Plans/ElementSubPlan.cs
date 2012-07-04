using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle
	{
		public Plan Plan { get; set; }

		[DataMember]
		public Guid PlanUID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementSubPlan()
			{
				Plan = Plan,
				PlanUID = PlanUID,
				Caption = Caption
			};
			Copy(elementBase);
			return elementBase;
		}

		public override FrameworkElement Draw()
		{
			var rectangle = new Rectangle()
			{
				Fill = new SolidColorBrush(BackgroundColor),
				Stroke = new SolidColorBrush(BorderColor),
				StrokeThickness = BorderThickness
			};

			if (BackgroundPixels != null)
				rectangle.Fill = PlanElementsHelper.CreateBrush(BackgroundPixels);

			return rectangle;
		}
	}
}