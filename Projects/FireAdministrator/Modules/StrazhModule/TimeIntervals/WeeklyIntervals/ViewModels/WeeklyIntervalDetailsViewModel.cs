﻿using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace StrazhModule.ViewModels
{
	public class WeeklyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalDetailsViewModel(SKDWeeklyInterval weeklyInterval)
		{
			Title = "Редактирование понедельногор графика";
			WeeklyInterval = weeklyInterval;
			Name = WeeklyInterval.Name;
			Description = WeeklyInterval.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever || Name == TimeIntervalsConfiguration.PredefinedIntervalNameAlways)
			{
				MessageBoxService.ShowWarning("Запрещенное назваине");
				return false;
			}
			WeeklyInterval.Name = Name;
			WeeklyInterval.Description = Description;
			return true;
		}
	}
}