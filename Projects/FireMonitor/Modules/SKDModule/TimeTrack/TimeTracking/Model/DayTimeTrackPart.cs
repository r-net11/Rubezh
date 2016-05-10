using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using ReactiveUI;
using ReactiveUI.Xaml;
using SKDModule.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace SKDModule.Model
{
	public class DayTimeTrackPart : ReactiveObject, IDataErrorInfo
	{
		#region Properties

		private IDisposable _subscriber;

		private Lazy<IObservable<Object>> _uiChanged;

		public IObservable<Object> UIChanged
		{
			get { return _uiChanged.Value; }
		}

		public bool IsRemoveAllIntersections { get; set; }

		public Guid? CorrectedByUID { get; set; }

		public Guid UID { get; set; }

		private bool _isManuallyAdded;

		public bool IsNew { get; set; }

		private bool _isDirty;

		public bool IsDirty
		{
			get { return _isDirty; }
			set { this.RaiseAndSetIfChanged(ref _isDirty, value); }
		}

		public bool IsManuallyAdded
		{
			get { return _isManuallyAdded; }
			set { this.RaiseAndSetIfChanged(ref _isManuallyAdded, value); }
		}

		private bool _notTakeInCalculations;

		public bool NotTakeInCalculations
		{
			get { return _notTakeInCalculations; }
			set { this.RaiseAndSetIfChanged(ref _notTakeInCalculations, value); }
		}

		public bool IsEnabledNotTakeInCalculations
		{
			get
			{
				if (TimeTrackZone != null && TimeTrackZone.IsURV && !IsOpen) return true;
				NotTakeInCalculations = true;
				return false;
			}
		}

		public bool NotTakeInCalculationsOriginal { get; set; }

		private bool _isOpen;

		public bool IsOpen
		{
			get { return _isOpen; }
			set
			{
				this.RaiseAndSetIfChanged(ref _isOpen, value);
			}
		}

		private bool _isNeedAdjustment;

		public bool IsNeedAdjustment
		{
			get { return _isNeedAdjustment; }
			set { this.RaiseAndSetIfChanged(ref _isNeedAdjustment, value); }
		}

		private DateTime? _enterDateTime;

		public DateTime? EnterDateTime
		{
			get { return _enterDateTime; }
			set { this.RaiseAndSetIfChanged(ref _enterDateTime, value); }
		}

		private TimeSpan _enterTime;

		public TimeSpan EnterTime
		{
			get { return _enterTime; }
			set { this.RaiseAndSetIfChanged(ref _enterTime, value); }
		}

		private DateTime? _exitDateTime;

		public DateTime? ExitDateTime
		{
			get { return _exitDateTime; }
			set { this.RaiseAndSetIfChanged(ref _exitDateTime, value); }
		}

		private TimeSpan _exitTime;

		public TimeSpan ExitTime
		{
			get { return _exitTime; }
			set { this.RaiseAndSetIfChanged(ref _exitTime, value); }
		}

		public bool IsValid
		{
			get { return string.IsNullOrEmpty(Error); }
		}

		public string Error
		{
			get
			{
				return this[string.Empty];
			}
		}

		private string _correctedBy;

		public string CorrectedBy
		{
			get { return _correctedBy; }
			set { this.RaiseAndSetIfChanged(ref _correctedBy, value); }
		}

		private TimeTrackZone _timeTrackZone;

		public TimeTrackZone TimeTrackZone
		{
			get { return _timeTrackZone; }
			set { this.RaiseAndSetIfChanged(ref _timeTrackZone, value); }
		}

		private bool _isForceClosed;

		public bool IsForceClosed
		{
			get { return _isForceClosed; }
			set { this.RaiseAndSetIfChanged(ref _isForceClosed, value); }
		}

		private DateTime? _enterTimeOriginal;

		public DateTime? EnterTimeOriginal
		{
			get { return _enterTimeOriginal; }
			set { this.RaiseAndSetIfChanged(ref _enterTimeOriginal, value); }
		}

		private DateTime? _exitTimeOriginal;

		public DateTime? ExitTimeOriginal
		{
			get { return _exitTimeOriginal; }
			set { this.RaiseAndSetIfChanged(ref _exitTimeOriginal, value); }
		}

		private DateTime? _adjustmentDate;

		public DateTime? AdjustmentDate
		{
			get { return _adjustmentDate; }
			set { this.RaiseAndSetIfChanged(ref _adjustmentDate, value); }
		}

		private bool _isNeedAdjustmentOriginal;

		public bool IsNeedAdjustmentOriginal
		{
			get { return _isNeedAdjustmentOriginal; }
			set { this.RaiseAndSetIfChanged(ref _isNeedAdjustmentOriginal, value); }
		}

		private TimeTrackActions _timeTrackActions;

		public TimeTrackActions TimeTrackActions
		{
			get { return _timeTrackActions; }
			set { this.RaiseAndSetIfChanged(ref _timeTrackActions, value); }
		}

		#endregion

		#region Constructors

		public DayTimeTrackPart(StrazhAPI.SKD.DayTimeTrackPart dayTimeTrackPart)
			: this()
		{
			if (dayTimeTrackPart == null) return;

			TimeTrackActions = dayTimeTrackPart.TimeTrackActions;
			IsRemoveAllIntersections = dayTimeTrackPart.IsRemoveAllIntersections;
			AdjustmentDate = dayTimeTrackPart.AdjustmentDate;
			CorrectedBy = dayTimeTrackPart.CorrectedBy;
			CorrectedByUID = dayTimeTrackPart.CorrectedByUID;
			//CorrectedDate = dayTimeTrackPart.AdjustmentDate.ToString();
			EnterDateTime = dayTimeTrackPart.EnterDateTime;
			EnterTime = dayTimeTrackPart.EnterTime;
			EnterTimeOriginal = dayTimeTrackPart.EnterTimeOriginal;
			ExitDateTime = dayTimeTrackPart.ExitDateTime;
			ExitTime = dayTimeTrackPart.ExitTime;
			ExitTimeOriginal = dayTimeTrackPart.ExitTimeOriginal;
			IsForceClosed = dayTimeTrackPart.IsForceClosed;
			IsManuallyAdded = dayTimeTrackPart.IsManuallyAdded;
			IsNeedAdjustment = dayTimeTrackPart.IsNeedAdjustment;
			IsNeedAdjustmentOriginal = dayTimeTrackPart.IsNeedAdjustmentOriginal;
			IsOpen = dayTimeTrackPart.IsOpen;
			NotTakeInCalculations = dayTimeTrackPart.NotTakeInCalculations;
			NotTakeInCalculationsOriginal = dayTimeTrackPart.NotTakeInCalculationsOriginal;
			if (dayTimeTrackPart.TimeTrackZone != null)
			{
				TimeTrackZone = new TimeTrackZone(dayTimeTrackPart.TimeTrackZone);
			}
			UID = dayTimeTrackPart.UID;
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart, ShortEmployee employee) : this()
		{
			var zone =
				TimeTrackingHelper.GetMergedZones(employee).FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID)
				??
				new TimeTrackZone {Name = "<Нет в конфигурации>", No = default(int)};

			UID = timeTrackPart.PassJournalUID;
			var user = timeTrackPart.IsManuallyAdded || timeTrackPart.AdjustmentDate != null
				? (FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.UID == timeTrackPart.CorrectedByUID)
				   ??
				   new User {Name = "<Нет в конфигурации>"})
				: new User {Name = string.Empty};

			Update(
				timeTrackPart.EnterDateTime,
				timeTrackPart.ExitDateTime,
				zone,
				timeTrackPart.NotTakeInCalculations,
				timeTrackPart.NotTakeInCalculationsOriginal,
				timeTrackPart.IsManuallyAdded,
				timeTrackPart.IsNeedAdjustment,
				timeTrackPart.IsNeedAdjustmentOriginal,
				timeTrackPart.AdjustmentDate,
				user.Name,
				user.UID,
				timeTrackPart.EnterTimeOriginal,
				timeTrackPart.ExitTimeOriginal,
				timeTrackPart.IsOpen,
				timeTrackPart.IsForceClosed);
		}

		public ReactiveAsyncCommand NotTakeIncalculationsCommand { get; set; }

		public DayTimeTrackPart()
		{
			_uiChanged = GetUiObserver();

			NotTakeIncalculationsCommand = new ReactiveAsyncCommand();
			NotTakeIncalculationsCommand.RegisterAsyncAction(i =>
			{
				var currentUser = FiresecManager.CurrentUser;
				var dateTimeNow = DateTime.Now;
				AdjustmentDate = dateTimeNow;
				CorrectedBy = currentUser.Name;
				CorrectedByUID = currentUser.UID;
				if (NotTakeInCalculations && ((TimeTrackActions & TimeTrackActions.Adding) != TimeTrackActions.Adding))
				{
					TimeTrackActions &= ~TimeTrackActions.TurnOffCalculation;
					TimeTrackActions |= TimeTrackActions.TurnOnCalculation;
				}
				else
				{
					TimeTrackActions &= ~TimeTrackActions.TurnOnCalculation;
					TimeTrackActions |= TimeTrackActions.TurnOffCalculation;
				}

			});
		}

		#endregion

		#region Methods

		public void Update(DateTime? enterDateTime, DateTime? exitDateTime,
			TimeTrackZone timeTrackZone, bool notTakeInCalculations, bool notTakeInCalculationsOriginal, bool isManuallyAdded, bool isNeedAdjustment, bool isNeedAdjustmentOriginal, DateTime? adjustmentDate, string correctedBy,
			Guid correctedByUID, DateTime? enterTimeOriginal, DateTime? exitTimeOriginal, bool isOpen = false, bool isForceClosed = false)
		{
			TimeTrackZone = timeTrackZone;
			AdjustmentDate = adjustmentDate;
			EnterDateTime = enterDateTime;
			EnterTime = enterDateTime.GetValueOrDefault().TimeOfDay;
			ExitDateTime = exitDateTime;
			ExitTime = exitDateTime.GetValueOrDefault().TimeOfDay;
			NotTakeInCalculations = notTakeInCalculations;
			NotTakeInCalculationsOriginal = notTakeInCalculationsOriginal;
			IsManuallyAdded = isManuallyAdded;
			IsNeedAdjustmentOriginal = isNeedAdjustmentOriginal;
			IsNeedAdjustment = isNeedAdjustment;
			CorrectedBy = correctedBy;
			CorrectedByUID = correctedByUID;
			IsOpen = isOpen;
			IsForceClosed = isForceClosed;
			EnterTimeOriginal = enterTimeOriginal;
			ExitTimeOriginal = exitTimeOriginal;
		}

		private Lazy<IObservable<object>> GetUiObserver()
		{
			return new Lazy<IObservable<object>>(() =>
				Observable.Merge<object>(
					this.ObservableForProperty(x => x.CorrectedBy)
					,this.ObservableForProperty(x => x.EnterDateTime)
					, this.ObservableForProperty(x => x.EnterTime)
					, this.ObservableForProperty(x => x.ExitDateTime)
					, this.ObservableForProperty(x => x.ExitTime)
					, this.ObservableForProperty(x => x.NotTakeInCalculations)
					, this.ObservableForProperty(x => x.IsManuallyAdded)
					, this.ObservableForProperty(x => x.IsNeedAdjustment)
					, this.ObservableForProperty(x => x.TimeTrackZone)
					, this.ObservableForProperty(x => x.IsRemoveAllIntersections)
					),
				true
				);
		}

		public StrazhAPI.SKD.DayTimeTrackPart ToDTO()
		{
			var timeTrackPart = new StrazhAPI.SKD.DayTimeTrackPart
			{
				UID = UID,
				TimeTrackActions = TimeTrackActions,
				AdjustmentDate = AdjustmentDate,
				CorrectedBy = CorrectedBy,
				CorrectedByUID = CorrectedByUID,
				EnterDateTime = EnterDateTime,
				EnterTime = EnterTime,
				EnterTimeOriginal = EnterTimeOriginal,
				ExitDateTime = ExitDateTime,
				ExitTime = ExitTime,
				ExitTimeOriginal = ExitTimeOriginal,
				IsForceClosed = IsForceClosed,
				IsManuallyAdded = IsManuallyAdded,
				IsNeedAdjustment = IsNeedAdjustment,
				IsNeedAdjustmentOriginal = IsNeedAdjustmentOriginal,
				IsOpen = IsOpen,
				IsNew = IsNew,
				IsRemoveAllIntersections = IsRemoveAllIntersections,
				NotTakeInCalculations = NotTakeInCalculations,
				NotTakeInCalculationsOriginal = NotTakeInCalculationsOriginal,
				TimeTrackZone = TimeTrackZone.ToDTO()
			};

			return timeTrackPart;
		}

		#endregion

		private string _currentError;
		public string CurrentError
		{
			get { return _currentError; }
			set { this.RaiseAndSetIfChanged(ref _currentError, value); }
		}

		public string this[string propertyName]
		{
			get
			{
				var result = string.Empty;
				propertyName = propertyName ?? string.Empty;

				if (propertyName == string.Empty || propertyName == "EnterTime" || propertyName == "ExitTime" || propertyName == "EnterDateTime" || propertyName == "ExitDateTime")
				{
					if ((EnterDateTime.HasValue && ExitDateTime.HasValue) && (EnterDateTime.Value.Date == ExitDateTime.Value.Date) && (EnterTime > ExitTime))
						result = "Время входа не может быть больше времени выхода";
				}
				if (propertyName == string.Empty || propertyName == "ExitTime" || propertyName == "EnterTime")
				{
					if ((EnterDateTime.HasValue && ExitDateTime.HasValue) && (EnterDateTime.Value.Date == ExitDateTime.Value.Date) && (EnterTime == ExitTime))
						result = "Невозможно добавить нулевое пребывание в зоне";
				}
				if (propertyName == string.Empty || propertyName == "EnterDateTime" || propertyName == "ExitDateTime")
				{
					if ((EnterDateTime.HasValue && ExitDateTime.HasValue) && (EnterDateTime.GetValueOrDefault().Date > ExitDateTime.GetValueOrDefault().Date))
						result = "Дата входа не может быть больше даты выхода";
				}
				if (propertyName == string.Empty || propertyName == "EnterDateTime" || propertyName == "ExitDateTime")
				{
					if (EnterDateTime == null || ExitDateTime == null)
						result = "Введите дату";
				}
				if (propertyName == string.Empty || propertyName == "EnterTime" || propertyName == "ExitTime" || propertyName == "ExitDateTime")
				{
					if (EnterDateTime.HasValue && ExitDateTime.HasValue && EnterDateTime.Value.Date == DateTime.Now.Date &&
					    ExitDateTime.Value.Date == DateTime.Now.Date)
					{
						if (ExitTime > DateTime.Now.TimeOfDay || EnterTime > DateTime.Now.TimeOfDay)
							result = "Дата не может быть установлена в будущее время";
					}
					if (EnterDateTime.HasValue && ExitDateTime.HasValue && EnterDateTime.Value.Date > DateTime.Now.Date ||
						ExitDateTime.GetValueOrDefault().Date > DateTime.Now.Date)
					{
						result = "Дата не может быть установлена в будущее время";
					}
				}

				CurrentError = result;
				return result;
			}
		}
	}
}
