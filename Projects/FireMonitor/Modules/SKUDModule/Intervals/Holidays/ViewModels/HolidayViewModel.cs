using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public EmployeeHoliday Holiday { get; set; }

		public HolidayViewModel(EmployeeHoliday holiday)
		{
			Holiday = holiday;
		}

		public void Update()
		{
			OnPropertyChanged("Holiday");
		}
	}
}