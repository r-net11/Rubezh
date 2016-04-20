using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<Schedule>
	{
		RubezhAPI.SKD.Organisation Organisation;
		IEnumerable<ScheduleScheme> _schemes;
		public Schedule Model { get; private set; }
		bool _isNew;
		public ScheduleDetailsViewModel() { }

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

		TimeSpan _allowedLate;
		public TimeSpan AllowedLate
		{
			get { return _allowedLate; }
			set
			{
				_allowedLate = value;
				OnPropertyChanged(() => AllowedLate);
			}
		}

		TimeSpan _allowedEarlyLeave;
		public TimeSpan AllowedEarlyLeave
		{
			get { return _allowedEarlyLeave; }
			set
			{
				_allowedEarlyLeave = value;
				OnPropertyChanged(() => AllowedEarlyLeave);
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
			Model.Name = Name;
			Model.IsIgnoreHoliday = IsIgnoreHoliday;
			Model.IsOnlyFirstEnter = IsOnlyFirstEnter;
			Model.AllowedLate = AllowedLate;
			Model.AllowedEarlyLeave = AllowedEarlyLeave;
			Model.ScheduleSchemeUID = SelectedScheduleScheme == null ? Guid.Empty : SelectedScheduleScheme.UID;
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return ScheduleHelper.Save(Model, _isNew);
		}

		public bool Initialize(Organisation organisation, Schedule model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_isNew = model == null;
			if (_isNew)
			{
				Title = "Новый график";
				Model = new Schedule()
				{
					Name = "Новый график работы",
					OrganisationUID = Organisation.UID,
				};
			}
			else
			{
				Title = "Редактирование графика работы";
				Model = ScheduleHelper.GetSingle(model.UID);
			}
			Name = Model.Name;
			IsIgnoreHoliday = Model.IsIgnoreHoliday;
			IsOnlyFirstEnter = Model.IsOnlyFirstEnter;
			AllowedLate = Model.AllowedLate;
			AllowedEarlyLeave = Model.AllowedEarlyLeave;
			AvailableScheduleTypes = new ObservableCollection<ScheduleSchemeType>(Enum.GetValues(typeof(ScheduleSchemeType)).OfType<ScheduleSchemeType>());
			_schemes = ScheduleSchemeHelper.Get(new ScheduleSchemeFilter()
			{
				OrganisationUIDs = new List<Guid>() { Organisation.UID },
				Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
				WithDays = false,
			});
			var selectedScheme = _schemes.FirstOrDefault(item => item.UID == Model.ScheduleSchemeUID);
			if (selectedScheme != null)
			{
				SelectedScheduleType = selectedScheme.Type;
				SelectedScheduleScheme = selectedScheme;
			}
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return true;
		}
	}
}