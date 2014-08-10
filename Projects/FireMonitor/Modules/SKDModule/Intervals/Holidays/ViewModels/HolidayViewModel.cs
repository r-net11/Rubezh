using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel : TreeNodeViewModel<HolidayViewModel>
	{
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Holiday Holiday { get; private set; }

		public HolidayViewModel(FiresecAPI.SKD.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public HolidayViewModel(FiresecAPI.SKD.Organisation organisation, Holiday holiday)
		{
			Organisation = organisation;
			Holiday = holiday;
			IsOrganisation = false;
			Name = holiday.Name;
		}

		public void Update(Holiday holiday)
		{
			Name = holiday.Name;
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => ReductionTime);
			OnPropertyChanged(() => TransitionDate);
		}

		public string ReductionTime
		{
			get
			{
				if (Holiday != null && Holiday.Type == HolidayType.BeforeHoliday)
					return Holiday.Reduction.ToString("hh\\-mm");
				return null;
			}
		}
		public string TransitionDate
		{
			get
			{
				if (Holiday != null && Holiday.Type == HolidayType.WorkingHoliday && Holiday.TransferDate.HasValue)
					return Holiday.TransferDate.Value.ToString("dd-MM");
				return null;
			}
		}
	}
}