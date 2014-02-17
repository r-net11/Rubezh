using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		MonthlyIntervalsViewModel MonthlyIntervalsViewModel;
		bool IsNew;
		public EmployeeMonthlyInterval MonthlyInterval { get; private set; }

		public MonthlyIntervalDetailsViewModel(MonthlyIntervalsViewModel monthlyIntervalsViewModel, EmployeeMonthlyInterval monthlyInterval = null)
		{
			MonthlyIntervalsViewModel = monthlyIntervalsViewModel;
			if (monthlyInterval == null)
			{
				Title = "Новый месячный график работы";
				IsNew = true;
				monthlyInterval = new EmployeeMonthlyInterval()
				{
					Name = "Месячный график работы"
				};
			}
			else
			{
				Title = "Редактирование месячного графика работы";
				IsNew = false;
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
			if (MonthlyIntervalsViewModel.MonthlyIntervals.Any(x => x.MonthlyInterval.Name == Name && x.MonthlyInterval.UID != MonthlyInterval.UID))
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