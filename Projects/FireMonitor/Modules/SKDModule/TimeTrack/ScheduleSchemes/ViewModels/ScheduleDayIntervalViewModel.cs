using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SheduleDayIntervalViewModel : BaseObjectViewModel<ScheduleDayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;
		public bool CanSelect { get { return !_scheduleScheme.IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit); } }

		public SheduleDayIntervalViewModel(ScheduleSchemeViewModel scheduleScheme, ScheduleDayInterval dayInterval)
			: base(dayInterval)
		{
			_initialized = false;
			_scheduleScheme = scheduleScheme;
			Update();
            if (_selectedDayInterval == null)
                _selectedDayInterval = DayIntervals.FirstOrDefault(x => x.Name == "Выходной");
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
			if (Model.DayIntervalUID != Guid.Empty)
			{
				var dayInterval = DayIntervals.SingleOrDefault(item => item.UID == Model.DayIntervalUID);
				if(dayInterval != null)
					_selectedDayInterval = dayInterval;
			}
				
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => DayIntervals);
			OnPropertyChanged(() => SelectedDayInterval);
			OnPropertyChanged(() => SelectedDayIntervalName);
		}

		DayInterval _selectedDayInterval;
		public DayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				SetDayInterval(value);
			}
		}

		public string SelectedDayIntervalName
		{
			get { return _selectedDayInterval != null ? _selectedDayInterval.Name : ""; }
		}

		public void SetDayInterval(DayInterval dayInterval)
		{
            if (!IsIntersection(dayInterval))
            {
                if (SelectedDayInterval != dayInterval)
                {
                    var selectedDayInterval = dayInterval ?? DayIntervals[0];
					if (_initialized || Model.DayIntervalUID != selectedDayInterval.UID)
                    {
						Model.DayIntervalUID = selectedDayInterval.UID;
						Model.DayIntervalName = selectedDayInterval.Name;
						if (_scheduleScheme.EditSave(Model))
						{
							_selectedDayInterval = selectedDayInterval;
						}
						else
						{
							Model.DayIntervalUID = _selectedDayInterval.UID;
							Model.DayIntervalName = _selectedDayInterval.Name;
						}
                    }
                }
				OnPropertyChanged(() => SelectedDayInterval);
				OnPropertyChanged(() => SelectedDayIntervalName);
            }
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

        bool IsIntersection (DayInterval dayInterval)
        {
            if (dayInterval.DayIntervalParts.Count > 0)
            {
                var yesterday = _scheduleScheme.Model.DayIntervals.FirstOrDefault(x => x.Number == Model.Number - 1);
                var tomorrow = _scheduleScheme.Model.DayIntervals.FirstOrDefault(x => x.Number == Model.Number + 1);
                if (yesterday != null)
                {
                    var yesterdayDayPartsNight = _scheduleScheme.DayIntervals.FirstOrDefault(x => x.UID == yesterday.DayIntervalUID).DayIntervalParts.Where(x => x.TransitionType == DayIntervalPartTransitionType.Night);
                    foreach (var item in yesterdayDayPartsNight)
                    {
                        if (item.EndTime > dayInterval.DayIntervalParts.Min(x => x.BeginTime))
                        {
                            MessageBoxService.ShowWarning("График имеет пересечение с интервалом предыдущего дня.");
                            return true;
                        }
                    }
                }
                if (tomorrow != null)
                {
                    var todayDayPartsNight = dayInterval.DayIntervalParts.Where(x => x.TransitionType == DayIntervalPartTransitionType.Night);
                    var tommorowDayParts = _scheduleScheme.DayIntervals.FirstOrDefault(x => x.UID == tomorrow.DayIntervalUID);
                    foreach (var item in todayDayPartsNight)
                    {
                        if (tommorowDayParts.DayIntervalParts.Count > 0 && item.EndTime > tommorowDayParts.DayIntervalParts.Min(x => x.BeginTime))
                        {
                            MessageBoxService.ShowWarning("График имеет пересечение с интервалом следующего дня.");
                            return true;
                        }
                    }
                }
            }
            return false;
        }
	}
}