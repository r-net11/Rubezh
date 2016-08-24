using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using EntitiesValidation;
using Localization.SKD.ViewModels;
using StrazhAPI;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SheduleDayIntervalViewModel : BaseObjectViewModel<ScheduleDayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;
		public bool CanSelect { get { return !_scheduleScheme.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit); } }

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
			get { return _selectedDayInterval.Name; }
		}

		public void SetDayInterval(DayInterval dayInterval)
		{
			var canSave = true;
			if (SelectedDayInterval != dayInterval)
			{
				_selectedDayInterval = dayInterval ?? DayIntervals[0];
				if (_initialized || Model.DayIntervalUID != _selectedDayInterval.UID)
				{
					Model.DayIntervalUID = _selectedDayInterval.UID;

					// Валидируем дневные графики на пересечение
					if (_scheduleScheme.Model.Type == ScheduleSchemeType.SlideDay)
					{
						var validationResult = ValidateDayIntervalIntersection(dayInterval);
						if (validationResult.HasError)
						{
							canSave = false;
							MessageBoxService.ShowWarning(String.Format("{0}\n\n{1}", CommonViewModels.NotSaveDaySchedule, validationResult.Error), null, 420, 200);
						}
					}
					
					// Сохраняем выбранный дневной график в БД
					if (canSave)
						SheduleDayIntervalHelper.Save(Model, _scheduleScheme.Name);
				}
			}
			OnPropertyChanged(() => SelectedDayInterval);
			OnPropertyChanged(() => SelectedDayIntervalName);
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

		private OperationResult ValidateDayIntervalIntersection(DayInterval dayInterval)
		{
			var dayIntervals = new List<DayInterval>();

			foreach (var sheduleDayInterval in _scheduleScheme.SheduleDayIntervals)
			{
				dayIntervals.Add(sheduleDayInterval == this ? dayInterval : sheduleDayInterval.SelectedDayInterval);
			}

			var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);
			if (validationResult.HasError)
				return new OperationResult(validationResult.Error);
			
			// Нет пересечений
			return new OperationResult();
		}
	}
}