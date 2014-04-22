using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.ScheduleShemes.ViewModels;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		private OrganisationSchedulesViewModel _organisationSchedulesViewModel;
		private IEnumerable<ScheduleScheme> _schemes;
		public Schedule Schedule { get; private set; }

		public ScheduleDetailsViewModel(OrganisationSchedulesViewModel organisationSchedulesViewModel, Schedule schedule = null)
		{
			_organisationSchedulesViewModel = organisationSchedulesViewModel;
			if (schedule == null)
			{
				Title = "Новый график";
				schedule = new Schedule()
				{
					Name = "Новый график работы",
					OrganisationUID = _organisationSchedulesViewModel.Organisation.UID,
				};
			}
			else
				Title = "Редактирование графика работы";
			Schedule = schedule;
			Name = Schedule.Name;
			IsIgnoreHoliday = Schedule.IsIgnoreHoliday;
			IsOnlyFirstEnter = Schedule.IsOnlyFirstEnter;
			AvailableScheduleTypes = new ObservableCollection<ScheduleSchemeType>(Enum.GetValues(typeof(ScheduleSchemeType)).OfType<ScheduleSchemeType>());
			_schemes = ScheduleSchemaHelper.Get(new ScheduleSchemeFilter()
			{
				OrganisationUIDs = new List<Guid>() { organisationSchedulesViewModel.Organisation.UID },
				Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
				WithDays = false,
			});
			var selectedScheme = _schemes.FirstOrDefault(item => item.UID == Schedule.ScheduleSchemeUID);
			if (selectedScheme != null)
			{
				SelectedScheduleType = selectedScheme.Type;
				SelectedScheduleScheme = selectedScheme;
			}
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

		private bool _isIgnoreHoliday;
		public bool IsIgnoreHoliday
		{
			get { return _isIgnoreHoliday; }
			set
			{
				_isIgnoreHoliday = value;
				OnPropertyChanged(() => IsIgnoreHoliday);
			}
		}

		private bool _isOnlyFirstEnter;
		public bool IsOnlyFirstEnter
		{
			get { return _isOnlyFirstEnter; }
			set
			{
				_isOnlyFirstEnter = value;
				OnPropertyChanged(() => IsOnlyFirstEnter);
			}
		}

		public ObservableCollection<ScheduleSchemeType> AvailableScheduleTypes { get; private set; }

		private ScheduleSchemeType _selectedScheduleType;
		public ScheduleSchemeType SelectedScheduleType
		{
			get { return _selectedScheduleType; }
			set
			{
				_selectedScheduleType = value;
				OnPropertyChanged(() => SelectedScheduleType);

				AvailableScheduleSchemes = new ObservableCollection<ScheduleScheme>(_schemes.Where(item => item.Type == SelectedScheduleType));
				SelectedScheduleScheme = null;
			}
		}

		private ObservableCollection<ScheduleScheme> _availableScheduleSchemes;
		public ObservableCollection<ScheduleScheme> AvailableScheduleSchemes
		{
			get { return _availableScheduleSchemes; }
			set
			{
				_availableScheduleSchemes = value;
				OnPropertyChanged(() => AvailableScheduleSchemes);
			}
		}

		private ScheduleScheme _selectedScheduleScheme;
		public ScheduleScheme SelectedScheduleScheme
		{
			get { return _selectedScheduleScheme; }
			set
			{
				_selectedScheduleScheme = value;
				OnPropertyChanged(() => SelectedScheduleScheme);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}
		protected override bool Save()
		{
			if (_organisationSchedulesViewModel.ViewModels.Any(x => x.Model.Name == Name && x.Model.UID != Schedule.UID))
			{
				MessageBoxService.ShowWarning("Название графика совпадает с введенным ранее");
				return false;
			}

			Schedule.Name = Name;
			Schedule.IsIgnoreHoliday = IsIgnoreHoliday;
			Schedule.IsOnlyFirstEnter = IsOnlyFirstEnter;
			Schedule.ScheduleSchemeUID = SelectedScheduleScheme == null ? Guid.Empty : SelectedScheduleScheme.UID;
			return true;
		}
	}
}