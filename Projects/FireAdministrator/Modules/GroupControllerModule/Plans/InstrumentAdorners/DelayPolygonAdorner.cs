using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DelayPolygonAdorner : BasePolygonAdorner
	{
		private DelaysViewModel _delaysViewModel;
		public DelayPolygonAdorner(CommonDesignerCanvas designerCanvas, DelaysViewModel delaysViewModel)
			: base(designerCanvas)
		{
			_delaysViewModel = delaysViewModel;
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
			var propertiesViewModel = new DelayPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_delaysViewModel.UpdateDelays(element.DelayUID);
				return element;
			}
			return null;
		}
	}
}