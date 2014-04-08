using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public Holiday Holiday { get; set; }

		public HolidayViewModel(Holiday holiday)
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
				if (Holiday.Type != HolidayType.Holiday)
					return Holiday.Reduction.ToString("HH-mm");
				return null;
			}
		}

		public string TransitionDateTime
		{
			get
			{
				if (Holiday.Type == HolidayType.WorkingHoliday)
					return Holiday.TransferDate.ToString("dd-MM");
				return null;
			}
		}
	}
}