using AutomationModule.Plans.ViewModels;
using AutomationModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace AutomationModule.Plans.InstrumentAdorners
{
	public class ProcedureRectangleAdorner : BaseRectangleAdorner
	{
		private ProceduresViewModel _proceduresViewModel;
		public ProcedureRectangleAdorner(CommonDesignerCanvas designerCanvas, ProceduresViewModel proceduresViewModel)
			: base(designerCanvas)
		{
			_proceduresViewModel = proceduresViewModel;
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementProcedure() { Left = left, Top = top };
			var propertiesViewModel = new ProcedurePropertiesViewModel(element, _proceduresViewModel);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}