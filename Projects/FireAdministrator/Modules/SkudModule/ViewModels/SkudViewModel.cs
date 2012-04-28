using Infrastructure;
using Infrastructure.Common;

namespace SkudModule.ViewModels
{
	public class SkudViewModel : RegionViewModel
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