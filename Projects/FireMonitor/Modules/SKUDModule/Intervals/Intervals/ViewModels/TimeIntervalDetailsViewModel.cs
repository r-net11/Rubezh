using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationTimeIntervalsViewModel OrganisationTimeIntervalsViewModel;
		public EmployeeTimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(OrganisationTimeIntervalsViewModel organisationTimeIntervalsViewModel, EmployeeTimeInterval timeInterval = null)
		{
			OrganisationTimeIntervalsViewModel = organisationTimeIntervalsViewModel;
			if (timeInterval == null)
			{
				Title = "Новый именованный интервал";
				timeInterval = new EmployeeTimeInterval()
				{
					Name = "Именованный интервал"
				};
				timeInterval.TimeIntervalParts.Add(new EmployeeTimeIntervalPart() { StartTime = new DateTime(2000, 1, 1, 9, 0, 0), EndTime = new DateTime(2000, 1, 1, 18, 0, 0) });
			}
			else
			{
				Title = "Редактирование именованного интервала";
			}
			TimeInterval = timeInterval;
			Name = TimeInterval.Name;
			Description = TimeInterval.Description;
			ConstantSlideTime = TimeInterval.ConstantSlideTime;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		DateTime _constantSlideTime;
		public DateTime ConstantSlideTime
		{
			get { return _constantSlideTime; }
			set
			{
				_constantSlideTime = value;
				OnPropertyChanged("ConstantSlideTime");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}

		protected override bool Save()
		{
			if (OrganisationTimeIntervalsViewModel.TimeIntervals.Any(x => x.TimeInterval.Name == Name && x.TimeInterval.UID != TimeInterval.UID))
			{
				MessageBoxService.ShowWarning("Название интервала совпадает с введенным ранее");
				return false;
			}

			TimeInterval.Name = Name;
			TimeInterval.Description = Description;
			TimeInterval.ConstantSlideTime = ConstantSlideTime;
			return true;
		}
	}
}