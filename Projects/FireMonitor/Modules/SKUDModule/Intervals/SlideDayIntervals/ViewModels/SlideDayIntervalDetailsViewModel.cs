using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		bool IsNew;
		public EmployeeSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalDetailsViewModel(SlideDayIntervalsViewModel slideDayIntervalsViewModel, EmployeeSlideDayInterval slideDayInterval = null)
		{
			SlideDayIntervalsViewModel = slideDayIntervalsViewModel;
			if (slideDayInterval == null)
			{
				Title = "Новый скользящий посуточный график";
				IsNew = true;
				slideDayInterval = new EmployeeSlideDayInterval()
				{
					Name = "Скользящий посуточный график"
				};
			}
			else
			{
				Title = "Редактирование скользящего посуточного графика";
				IsNew = false;
			}
			SlideDayInterval = slideDayInterval;
			Name = SlideDayInterval.Name;
			Description = SlideDayInterval.Description;
			StartDate = SlideDayInterval.StartDate;
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

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged("StartDate");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Доступ запрещен";
		}

		protected override bool Save()
		{
			if (SlideDayIntervalsViewModel.SlideDayIntervals.Any(x => x.SlideDayInterval.Name == Name && x.SlideDayInterval.UID != SlideDayInterval.UID))
			{
				MessageBoxService.ShowWarning("Название интервала совпадает с введенным ранее");
				return false;
			}

			SlideDayInterval.Name = Name;
			SlideDayInterval.Description = Description;
			SlideDayInterval.StartDate = StartDate;
			return true;
		}
	}
}