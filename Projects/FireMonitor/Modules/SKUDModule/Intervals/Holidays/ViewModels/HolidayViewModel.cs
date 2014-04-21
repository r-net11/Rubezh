using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel : BaseObjectViewModel<Holiday>
	{
		public HolidayViewModel(Holiday holiday)
			: base(holiday)
		{
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => ReductionTime);
			OnPropertyChanged(() => TransitionDate);
		}

		public string ReductionTime
		{
			get
			{
				if (Model.Type != HolidayType.Holiday)
					return Model.Reduction.ToString("hh\\-mm");
				return null;
			}
		}
		public string TransitionDate
		{
			get
			{
				if (Model.Type == HolidayType.WorkingHoliday && Model.TransferDate.HasValue)
					return Model.TransferDate.Value.ToString("dd-MM");
				return null;
			}
		}
	}
}