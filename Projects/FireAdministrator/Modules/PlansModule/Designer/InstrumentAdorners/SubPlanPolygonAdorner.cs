using System.Windows.Media;
using System.Windows.Shapes;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using PlansModule.ViewModels;
using RubezhAPI.Models;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanPolygonAdorner : BasePolygonAdorner
	{
		public SubPlanPolygonAdorner(DesignerCanvas designerCanvas)
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

		protected override Infrustructure.Plans.Elements.ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonSubPlan();
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}