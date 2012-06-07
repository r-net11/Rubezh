using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace SkudModule.ViewModels
{
	public class SkudViewModel : ViewPartViewModel
	{
		public SkudViewModel()
		{
		}

		public void Initialize()
		{

		}

		public override void OnShow()
		{
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);
		}
	}
}