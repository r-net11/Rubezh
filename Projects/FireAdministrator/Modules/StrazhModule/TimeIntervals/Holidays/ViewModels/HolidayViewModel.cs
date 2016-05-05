using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public SKDHoliday Holiday { get; set; }

		public HolidayViewModel(SKDHoliday holiday)
		{
			Holiday = holiday;
		}

		public void Update()
		{
			OnPropertyChanged(() => Holiday);
		}
	}
}