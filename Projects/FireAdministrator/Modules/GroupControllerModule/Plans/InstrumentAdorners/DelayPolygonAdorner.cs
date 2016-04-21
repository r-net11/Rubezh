using System.Windows.Media;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DelayPolygonAdorner : BasePolygonAdorner
	{
		public DelayPolygonAdorner(CommonDesignerCanvas designerCanvas, DelaysViewModel delaysViewModel)
			: base(designerCanvas)
		{
			this._delaysViewModel = delaysViewModel;
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonGKDelay();
			var propertiesViewModel = new DelayPropertiesViewModel(element, _delaysViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKDelay>(element);
			return element;
		}

		#region Fields
		private DelaysViewModel _delaysViewModel;

		#endregion
	}
}
