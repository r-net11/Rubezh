using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackingSettingsViewModel : SaveCancelDialogViewModel
	{
		private TimeTrackingSettings _settings;
		public TimeTrackingSettingsViewModel(TimeTrackingSettings settings)
		{
			Title = "Настройка отчета";
			_settings = settings;

			Periods = new ObservableCollection<TimeTrackingPeriod>(Enum.GetValues(typeof(TimeTrackingPeriod)).OfType<TimeTrackingPeriod>());
			Period = _settings.Period;
			StartDate = _settings.StartDate;
			EndDate = _settings.EndDate;
		}

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
			}
		}

		private DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged(() => EndDate);
			}
		}

		public ObservableCollection<TimeTrackingPeriod> Periods { get; private set; }

		private TimeTrackingPeriod _period;
		public TimeTrackingPeriod Period
		{
			get { return _period; }
			set
			{
				_period = value;
				OnPropertyChanged(() => Period);
				OnPropertyChanged(() => IsFreePeriod);
			}
		}

		public bool IsFreePeriod
		{
			get { return Period == TimeTrackingPeriod.Period; }
		}

		protected override bool CanSave()
		{
			return base.CanSave();
		}
		protected override bool Save()
		{
			if (Period == TimeTrackingPeriod.Period && StartDate >= EndDate)
			{
				MessageBoxService.ShowWarning("Дата окончания не может быть раньше даты начала");
				return false;
			}

			_settings.Period = Period;
			switch (Period)
			{
				case TimeTrackingPeriod.CurrentMonth:
					_settings.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					_settings.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.CurrentWeek:
					_settings.StartDate = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					_settings.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.PreviosMonth:
					var firstMonthDay = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					_settings.StartDate = firstMonthDay.AddMonths(-1);
					_settings.EndDate = firstMonthDay.AddDays(-1);
					break;
				case TimeTrackingPeriod.PreviosWeek:
					var firstWeekDay = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					_settings.StartDate = firstWeekDay.AddDays(-7);
					_settings.EndDate = firstWeekDay.AddDays(-1);
					break;
				case TimeTrackingPeriod.Period:
					_settings.StartDate = StartDate;
					_settings.EndDate = EndDate;
					break;
			}
			return true;
		}
	}
}