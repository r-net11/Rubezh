using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class NamedIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		private OrganisationNamedIntervalsViewModel _organisationNamedIntervalsViewModel;
		public NamedInterval NamedInterval { get; private set; }

		public NamedIntervalDetailsViewModel(OrganisationNamedIntervalsViewModel organisationNameIntervalsViewModel, NamedInterval namedInterval = null)
		{
			_organisationNamedIntervalsViewModel = organisationNameIntervalsViewModel;
			if (namedInterval == null)
			{
				Title = "Новый именованный интервал";
				namedInterval = new NamedInterval()
				{
					Name = "Именованный интервал",
					OrganisationUID = organisationNameIntervalsViewModel.Organization.UID,
				};
				namedInterval.TimeIntervals.Add(new TimeInterval() { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), NamedIntervalUID = namedInterval.UID });
			}
			else
				Title = "Редактирование именованного интервала";
			NamedInterval = namedInterval;
			Name = NamedInterval.Name;
			Description = NamedInterval.Description;
			ConstantSlideTime = NamedInterval.SlideTime;
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private TimeSpan _constantSlideTime;
		public TimeSpan ConstantSlideTime
		{
			get { return _constantSlideTime; }
			set
			{
				_constantSlideTime = value;
				OnPropertyChanged(() => ConstantSlideTime);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}
		protected override bool Save()
		{
			if (_organisationNamedIntervalsViewModel.ViewModels.Any(x => x.Model.Name == Name && x.Model.UID != NamedInterval.UID))
			{
				MessageBoxService.ShowWarning("Название интервала совпадает с введенным ранее");
				return false;
			}

			NamedInterval.Name = Name;
			NamedInterval.Description = Description;
			NamedInterval.SlideTime = ConstantSlideTime;
			return true;
		}
	}
}