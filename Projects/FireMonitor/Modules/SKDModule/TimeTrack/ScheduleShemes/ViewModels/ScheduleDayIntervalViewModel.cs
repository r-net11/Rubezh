using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SheduleDayIntervalViewModel : BaseObjectViewModel<ScheduleDayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;
		public bool CanSelect { get { return !_scheduleScheme.IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit); } }

		public SheduleDayIntervalViewModel(ScheduleSchemeViewModel scheduleScheme, ScheduleDayInterval dayInterval)
			: base(dayInterval)
		{
			_initialized = false;
			_scheduleScheme = scheduleScheme;
			Update();
			_scheduleScheme.PropertyChanged += OrganisationViewModel_PropertyChanged;
			_initialized = true;
		}

		public string Name { get; private set; }
		public ObservableCollection<DayInterval> DayIntervals
		{
			get { return _scheduleScheme.DayIntervals; }
		}

		public override void Update()
		{
			base.Update();
			if (_scheduleScheme.Model.Type == ScheduleSchemeType.Week)
			{
				var dayOfWeek = (DayOfWeek)((Model.Number + 1) % 7);
				var culture = new System.Globalization.CultureInfo("ru-RU");
				Name = culture.DateTimeFormat.GetDayName(dayOfWeek);
			}
			else
			{
				Name = (Model.Number + 1).ToString();
			}
			_selectedDayInterval = DayIntervals.SingleOrDefault(item => item.UID == Model.DayIntervalUID) ?? DayIntervals[0];
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => DayIntervals);
			OnPropertyChanged(() => SelectedDayInterval);
		}

		DayInterval _selectedDayInterval;
		public DayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				SetDayInterval(value, _scheduleScheme.Model.Type == ScheduleSchemeType.Week && IsWorkDay(Model));
			}
		}

		public void SetDayInterval(DayInterval dayInterval, bool isWithOthers)
		{
			if (SelectedDayInterval != dayInterval)
			{
				_selectedDayInterval = dayInterval ?? DayIntervals[0];
				if (_initialized || Model.DayIntervalUID != _selectedDayInterval.UID)
				{
					Model.DayIntervalUID = _selectedDayInterval.UID;
					SheduleDayIntervalHelper.Save(Model, _scheduleScheme.Name);
				}
			}
			if (isWithOthers)
			{
				_scheduleScheme.SheduleDayIntervals.Where(x => IsWorkDay(x.Model)).ForEach(x => x.SetDayInterval(dayInterval, false));
			}
			OnPropertyChanged(() => SelectedDayInterval);
		}

		bool IsWorkDay(ScheduleDayInterval dayInterval)
		{
			return dayInterval.Number >= 0 && dayInterval.Number <= 4;
		}

		void OrganisationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DayIntervals")
				Update();
		}
	}
}