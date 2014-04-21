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
			OnPropertyChanged(() => Holiday);
			OnPropertyChanged(() => ReductionTime);
			OnPropertyChanged(() => TransitionDate);
		}

		public string ReductionTime
		{
			get
			{
				if (Holiday.Type != HolidayType.Holiday)
					return Holiday.Reduction.ToString("hh\\-mm");
				return null;
			}
		}

		public string TransitionDate
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