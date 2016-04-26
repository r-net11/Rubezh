using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DelayRectangleAdorner : BaseRectangleAdorner
	{
		private DelaysViewModel _delaysViewModel;
		public DelayRectangleAdorner(CommonDesignerCanvas designerCanvas, DelaysViewModel delayViewModel)
			: base(designerCanvas)
		{
			_delaysViewModel = delayViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKDelay();
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