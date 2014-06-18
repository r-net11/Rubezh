using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public CardsViewModel CardsViewModel { get; private set; }
		public CardRecsViewModel CardRecsViewModel { get; private set; }
		public PasswordsViewModel PasswordsViewModel { get; private set; }
		public HolidaysViewModel HolidaysViewModel { get; private set; }
		public TimeShedulesViewModel TimeShedulesViewModel { get; private set; }
		public ControlViewModel ControlViewModel { get; private set; }

		public MainViewModel()
		{
			CardsViewModel = new CardsViewModel();
			CardRecsViewModel = new CardRecsViewModel();
			PasswordsViewModel = new PasswordsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			TimeShedulesViewModel = new TimeShedulesViewModel();
			ControlViewModel = new ControlViewModel();
		}
	}
}