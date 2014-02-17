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
			OnPropertyChanged("ShortageTime");
			OnPropertyChanged("TransitionDateTime");
		}

		public string ShortageTime
		{
			get
			{
				if (Holiday.EmployeeHolidayType != EmployeeHolidayType.Holiday)
					return Holiday.ShortageTime.ToString("HH-mm");
				return null;
			}
		}

		public string TransitionDateTime
		{
			get
			{
				if (Holiday.EmployeeHolidayType == EmployeeHolidayType.WorkingHoliday)
					return Holiday.TransitionDateTime.ToString("dd-MM");
				return null;
			}
		}
	}
}