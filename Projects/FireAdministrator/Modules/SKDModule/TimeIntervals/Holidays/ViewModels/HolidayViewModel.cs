using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
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