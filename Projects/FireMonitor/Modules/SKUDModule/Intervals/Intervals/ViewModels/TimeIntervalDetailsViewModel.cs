using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationTimeIntervalsViewModel OrganisationTimeIntervalsViewModel;
		public NamedInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(OrganisationTimeIntervalsViewModel organisationTimeIntervalsViewModel, NamedInterval timeInterval = null)
		{
			OrganisationTimeIntervalsViewModel = organisationTimeIntervalsViewModel;
			if (timeInterval == null)
			{
				Title = "Новый именованный интервал";
				timeInterval = new NamedInterval()
				{
					Name = "Именованный интервал"
				};
				timeInterval.TimeIntervals.Add(new TimeInterval() { StartTime = new DateTime(2000, 1, 1, 9, 0, 0), EndTime = new DateTime(2000, 1, 1, 18, 0, 0) });
			}
			else
			{
				Title = "Редактирование именованного интервала";
			}
			TimeInterval = timeInterval;
			Name = TimeInterval.Name;
			Description = TimeInterval.Description;
			ConstantSlideTime = TimeInterval.SlideTime;
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

		TimeSpan _constantSlideTime;
		public TimeSpan ConstantSlideTime
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
			TimeInterval.SlideTime = ConstantSlideTime;
			return true;
		}
	}
}