using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using PlansModule.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanPolygonAdorner : BasePolygonAdorner
	{
		public SubPlanPolygonAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}

		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}

		protected override ElementBaseShape CreateElement(RubezhAPI.PointCollection points)
		{
			var element = new ElementPolygonSubPlan { Points = points };
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}