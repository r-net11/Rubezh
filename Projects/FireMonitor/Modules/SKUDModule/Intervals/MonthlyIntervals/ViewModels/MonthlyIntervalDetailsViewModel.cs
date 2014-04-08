using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Organization = FiresecAPI.Organization;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationMonthlyIntervalsViewModel OrganisationMonthlyIntervalsViewModel;
		public ScheduleScheme MonthlyInterval { get; private set; }

		public MonthlyIntervalDetailsViewModel(OrganisationMonthlyIntervalsViewModel organisationMonthlyIntervalsViewModel, ScheduleScheme monthlyInterval = null)
		{
			OrganisationMonthlyIntervalsViewModel = organisationMonthlyIntervalsViewModel;
			if (monthlyInterval == null)
			{
				Title = "Новый месячный график работы";
				monthlyInterval = new ScheduleScheme()
				{
					Name = "Месячный график работы"
				};
			}
			else
			{
				Title = "Редактирование месячного графика работы";
			}
			MonthlyInterval = monthlyInterval;
			Name = MonthlyInterval.Name;
			Description = MonthlyInterval.Description;
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

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Доступ запрещен";
		}

		protected override bool Save()
		{
			if (OrganisationMonthlyIntervalsViewModel.MonthlyIntervals.Any(x => x.MonthlyInterval.Name == Name && x.MonthlyInterval.UID != MonthlyInterval.UID))
			{
				MessageBoxService.ShowWarning("Название графика совпадает с введенным ранее");
				return false;
			}

			MonthlyInterval.Name = Name;
			MonthlyInterval.Description = Description;
			return true;
		}
	}
}