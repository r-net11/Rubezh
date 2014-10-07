using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SheduleDayIntervalViewModel : BaseObjectViewModel<ScheduleDayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;
		public bool CanSelect { get { return !_scheduleScheme.IsDeleted; } }

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
			//Name = _scheduleScheme.ScheduleScheme.Type == ScheduleSchemeType.Week ? ((DayOfWeek)((Model.Number + 1) % 7)).ToString("dddd") : (Model.Number + 1).ToString();
			SelectedDayInterval = DayIntervals.SingleOrDefault(item => item.UID == Model.DayIntervalUID) ?? DayIntervals[0];
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => DayIntervals);
		}

		DayInterval _selectedDayInterval;
		public DayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				if (SelectedDayInterval != value)
				{
					_selectedDayInterval = value ?? DayIntervals[0];
					if (_initialized || Model.DayIntervalUID != _selectedDayInterval.UID)
					{
						Model.DayIntervalUID = _selectedDayInterval.UID;
						SheduleDayIntervalHelper.Save(Model);
					}
				}
				OnPropertyChanged(() => SelectedDayInterval);
			}
		}

		void OrganisationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DayIntervals")
				Update();
		}
	}
}