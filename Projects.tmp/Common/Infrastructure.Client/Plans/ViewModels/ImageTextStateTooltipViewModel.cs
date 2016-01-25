using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Plans.ViewModels
{
	public class ImageTextStateTooltipViewModel : BaseViewModel
	{
		public ImageTextStateTooltipViewModel()
		{
			TitleViewModel = new ImageTextTooltipViewModel();
			StateViewModel = new ImageTextTooltipViewModel();
		}

		public ImageTextTooltipViewModel TitleViewModel { get; private set; }
		public ImageTextTooltipViewModel StateViewModel { get; private set; }

		public void Update()
		{
			OnPropertyChanged(() => TitleViewModel);
			OnPropertyChanged(() => StateViewModel);
		}
	}
}