using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		FiresecAPI.SKD.Organisation Organisation;
		IEnumerable<ScheduleScheme> _schemes;
		public Schedule Schedule { get; private set; }

		public ScheduleDetailsViewModel(FiresecAPI.SKD.Organisation organisation, Schedule schedule = null)
		{
			Organisation = organisation;
			if (schedule == null)
			{
				Title = "Новый график";
				schedule = new Schedule()
				{
					Name = "Новый график работы",
					OrganisationUID = Organisation.UID,
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
				OrganisationUIDs = new List<Guid>() { Organisation.UID },
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

		bool _isIgnoreHoliday;
		public bool IsIgnoreHoliday
		{
			get { return _isIgnoreHoliday; }
			set
			{
				_isIgnoreHoliday = value;
				OnPropertyChanged(() => IsIgnoreHoliday);
			}
		}

		bool _isOnlyFirstEnter;
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

		ScheduleSchemeType _selectedScheduleType;
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

		ObservableCollection<ScheduleScheme> _availableScheduleSchemes;
		public ObservableCollection<ScheduleScheme> AvailableScheduleSchemes
		{
			get { return _availableScheduleSchemes; }
			set
			{
				_availableScheduleSchemes = value;
				OnPropertyChanged(() => AvailableScheduleSchemes);
			}
		}

		ScheduleScheme _selectedScheduleScheme;
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
			Schedule.Name = Name;
			Schedule.IsIgnoreHoliday = IsIgnoreHoliday;
			Schedule.IsOnlyFirstEnter = IsOnlyFirstEnter;
			Schedule.ScheduleSchemeUID = SelectedScheduleScheme == null ? Guid.Empty : SelectedScheduleScheme.UID;
			return ScheduleHelper.Save(Schedule);
			;
		}
	}
}