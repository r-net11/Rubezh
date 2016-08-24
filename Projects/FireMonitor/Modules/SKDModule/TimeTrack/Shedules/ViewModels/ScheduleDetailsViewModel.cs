using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;

namespace SKDModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<Schedule>
	{
		#region Fields

		private const int DefaultMinutesValue = 5;
		Organisation Organisation;
		IEnumerable<ScheduleScheme> _schemes;
		bool _isNew;
		#endregion

		#region Properties
		public ObservableCollection<ScheduleSchemeType> AvailableScheduleTypes { get; private set; }

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

		private bool _isIgnoreHolidayEnabled;
		public bool IsIgnoreHolydayEnabled
		{
			get { return _isIgnoreHolidayEnabled; }
			set
			{
				if (value == _isIgnoreHolidayEnabled) return;
				_isIgnoreHolidayEnabled = value;
				OnPropertyChanged(() => IsIgnoreHolydayEnabled);
			}
		}

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

		public Schedule Model { get; private set; }

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

		private bool _isEnabledAllowLate;

		public bool IsEnabledAllowLate
		{
			get { return _isEnabledAllowLate; }
			set
			{
				_isEnabledAllowLate = value;
				OnPropertyChanged(() => IsEnabledAllowLate);
			}
		}

		private bool _isAllowAbsent;

		public bool IsAllowAbsent
		{
			get { return _isAllowAbsent; }
			set
			{
				_isAllowAbsent = value;
				OnPropertyChanged(() => IsAllowAbsent);
			}
		}

		private bool _isEnabledAllowEarlyLeave;

		public bool IsEnabledAllowEarlyLeave
		{
			get { return _isEnabledAllowEarlyLeave; }
			set
			{
				_isEnabledAllowEarlyLeave = value;
				OnPropertyChanged(() => IsEnabledAllowEarlyLeave);
			}
		}

		private bool _isEnabledOvertime;

		public bool IsEnabledOvertime
		{
			get { return _isEnabledOvertime; }
			set
			{
				_isEnabledOvertime = value;
				OnPropertyChanged(() => IsEnabledOvertime);
			}
		}

		private int _notAllowOvertimeLowerThan;

		public int NotAllowOvertimeLowerThan
		{
			get { return _notAllowOvertimeLowerThan; }
			set
			{
				_notAllowOvertimeLowerThan = value;
				OnPropertyChanged(() => NotAllowOvertimeLowerThan);
			}
		}

		private int _allowedEarlyLeave;
		public int AllowedEarlyLeave
		{
			get { return _allowedEarlyLeave; }
			set
			{
				_allowedEarlyLeave = value;
				OnPropertyChanged(() => AllowedEarlyLeave);
			}
		}

		private int _allowedAbsentLowThan;

		public int AllowedAbsentLowThan
		{
			get { return _allowedAbsentLowThan; }
			set
			{
				_allowedAbsentLowThan = value;
				OnPropertyChanged(() => AllowedAbsentLowThan);
			}
		}

		private int _allowedLate;
		public int AllowedLate
		{
			get { return _allowedLate; }
			set
			{
				_allowedLate = value;
				OnPropertyChanged(() => AllowedLate);
			}
		}

		#endregion

		public ScheduleDetailsViewModel()
		{
			this.WhenAny(x => x.SelectedScheduleType, x => x.Value)
				.Subscribe(value =>
				{
					if (value != ScheduleSchemeType.Week)
					{
						IsIgnoreHolydayEnabled = false;
						IsIgnoreHoliday = true;
					}
					else
					{
						IsIgnoreHolydayEnabled = true;
						IsIgnoreHoliday = Model.IsIgnoreHoliday;
					}
				});
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
			Model.ScheduleSchemeUID = SelectedScheduleScheme == null ? Guid.Empty : SelectedScheduleScheme.UID;

			Model.AllowedLate = IsEnabledAllowLate ? AllowedLate : default(int);
			Model.AllowedEarlyLeave = IsEnabledAllowEarlyLeave ? AllowedEarlyLeave : default(int);
			Model.AllowedAbsentLowThan = IsAllowAbsent ? AllowedAbsentLowThan : default(int);
			Model.NotAllowOvertimeLowerThan = IsEnabledOvertime ? NotAllowOvertimeLowerThan : default(int);
			Model.IsEnabledAllowLate = IsEnabledAllowLate;
			Model.IsEnabledAllowEarlyLeave = IsEnabledAllowEarlyLeave;
			Model.IsAllowAbsent = IsAllowAbsent;
			Model.IsEnabledOvertime = IsEnabledOvertime;

			return ScheduleHelper.Save(Model, _isNew);
		}

		private void InitializeForNewSchedule()
		{
			Title = CommonViewModels.NewSchedule;
			Model = new Schedule
			{
				Name = CommonViewModels.NewWorkSchedule,
				OrganisationUID = Organisation.UID,
			};

			AllowedLate = DefaultMinutesValue;
			AllowedAbsentLowThan = DefaultMinutesValue;
			AllowedEarlyLeave = DefaultMinutesValue;
			NotAllowOvertimeLowerThan = DefaultMinutesValue;
		}

		private void InitializeForEditSchedule(Schedule model)
		{
			Title = CommonViewModels.WorkScheduleEdition;
			Model = ScheduleHelper.GetSingle(model.UID);
			AllowedLate = Model.IsEnabledAllowLate ? Model.AllowedLate : DefaultMinutesValue;
			AllowedAbsentLowThan = Model.IsAllowAbsent ? Model.AllowedAbsentLowThan : DefaultMinutesValue;
			AllowedEarlyLeave = Model.IsEnabledAllowEarlyLeave ? Model.AllowedEarlyLeave : DefaultMinutesValue;
			NotAllowOvertimeLowerThan = Model.IsEnabledOvertime ? Model.NotAllowOvertimeLowerThan : DefaultMinutesValue;

			IsEnabledAllowLate = Model.IsEnabledAllowLate;
			IsEnabledAllowEarlyLeave = Model.IsEnabledAllowEarlyLeave;
			IsAllowAbsent = Model.IsAllowAbsent;
			IsEnabledOvertime = Model.IsEnabledOvertime;
		}

		public bool Initialize(Organisation organisation, Schedule model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_isNew = model == null;
			if (_isNew)
			{
				InitializeForNewSchedule();
			}
			else
			{
				InitializeForEditSchedule(model);
			}
			Name = Model.Name;
			IsIgnoreHoliday = Model.IsIgnoreHoliday;
			IsOnlyFirstEnter = Model.IsOnlyFirstEnter;
			AvailableScheduleTypes = new ObservableCollection<ScheduleSchemeType>(Enum.GetValues(typeof(ScheduleSchemeType)).OfType<ScheduleSchemeType>());
			_schemes = ScheduleSchemeHelper.Get(new ScheduleSchemeFilter
			{
				OrganisationUIDs = new List<Guid> { Organisation.UID },
				Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
				WithDays = false,
			});
			var selectedScheme = _schemes.FirstOrDefault(item => item.UID == Model.ScheduleSchemeUID);

			if (selectedScheme == null) return true;

			SelectedScheduleType = selectedScheme.Type;
			SelectedScheduleScheme = selectedScheme;

			return true;
		}
	}
}