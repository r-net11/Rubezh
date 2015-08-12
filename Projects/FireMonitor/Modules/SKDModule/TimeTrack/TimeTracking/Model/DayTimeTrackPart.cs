using System.Globalization;
using System.Reactive.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using ReactiveUI;
using SKDModule.Helpers;
using SKDModule.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

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
				if (TimeTrackZone != null && TimeTrackZone.IsURV) return true;
				NotTakeInCalculations = true;
				return false;
			}
		}

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

		private string _correctedDate;
		public string CorrectedDate
		{
			get { return _correctedDate; }
			set { this.RaiseAndSetIfChanged(ref _correctedDate, value); }
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

		#endregion

		#region Constructors

		public DayTimeTrackPart(TimeTrackPartDetailsViewModel timeTrackPartDetailsViewModel) : this()
		{
			//UID = timeTrackPartDetailsViewModel.UID;
			//Update(
			//	timeTrackPartDetailsViewModel.EnterDateTime + timeTrackPartDetailsViewModel.EnterTime,
			//	timeTrackPartDetailsViewModel.EnterDateTime + timeTrackPartDetailsViewModel.ExitTime,
			//	timeTrackPartDetailsViewModel.SelectedZone,
			//	timeTrackPartDetailsViewModel.NotTakeInCalculations,
			//	timeTrackPartDetailsViewModel.IsManuallyAdded,
			//	DateTime.Now.ToString(CultureInfo.CurrentUICulture),
			//	FiresecManager.CurrentUser.Name);
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart, ShortEmployee employee) : this()
		{
			var zone =
				TimeTrackingHelper.GetMergedZones(employee).FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID)
				??
				new TimeTrackZone {Name = "<Нет в конфигурации>", No = default(int)};

			UID = timeTrackPart.PassJournalUID;
			var user = timeTrackPart.IsManuallyAdded
				? (FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.UID == timeTrackPart.CorrectedByUID)
					??
					new User {Name = "<Нет в конфигурации>"})
				: new User {Name = string.Empty};

			Update(
				timeTrackPart.EnterDateTime,
				timeTrackPart.ExitDateTime,
				zone,
				timeTrackPart.NotTakeInCalculations,
				timeTrackPart.IsManuallyAdded,
				timeTrackPart.AdjustmentDate.ToString(),
				user.Name,
				timeTrackPart.EnterTimeOriginal,
				timeTrackPart.ExitTimeOriginal,
				timeTrackPart.IsOpen,
				timeTrackPart.IsForceClosed);
		}

		public DayTimeTrackPart()
		{
			_uiChanged = GetUiObserver();
		}

		#endregion

		#region Methods

		public void Update(DateTime? enterDateTime, DateTime? exitDateTime,
			TimeTrackZone timeTrackZone, bool notTakeInCalculations, bool isManuallyAdded, string adjustmentDate, string correctedBy, DateTime? enterTimeOriginal, DateTime? exitTimeOriginal, bool isOpen = false, bool isForceClosed = false)
		{
			TimeTrackZone = timeTrackZone;
			EnterDateTime = enterDateTime;
			EnterTime = enterDateTime.GetValueOrDefault().TimeOfDay;
			ExitDateTime = exitDateTime;
			ExitTime = exitDateTime.GetValueOrDefault().TimeOfDay;
			NotTakeInCalculations = notTakeInCalculations;
			IsManuallyAdded = isManuallyAdded;
			CorrectedDate = adjustmentDate;
			CorrectedBy = correctedBy;
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
					, this.ObservableForProperty(x => x.CorrectedDate)
					, this.ObservableForProperty(x => x.EnterDateTime)
					, this.ObservableForProperty(x => x.EnterTime)
					, this.ObservableForProperty(x => x.ExitDateTime)
					, this.ObservableForProperty(x => x.ExitTime)
					, this.ObservableForProperty(x => x.IsManuallyAdded)
					, this.ObservableForProperty(x => x.IsNeedAdjustment)
					, this.ObservableForProperty(x => x.TimeTrackZone)
					),
				true
				);
		}

		#endregion

		private string _currentError;
		public string CurrentError
		{
			get { return _currentError; }
			set
			{
				this.RaiseAndSetIfChanged(ref _currentError, value);
			}
		}

		public string this[string propertyName]
		{
			get
			{
				var result = string.Empty;
				propertyName = propertyName ?? string.Empty;
				if (propertyName == string.Empty || propertyName == "EnterTime" || propertyName == "ExitTime")
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
					if ((EnterDateTime.HasValue && ExitDateTime.HasValue) && (EnterDateTime.Value.Date == ExitDateTime.Value.Date) && (EnterDateTime > ExitDateTime))
						result = "Дата входа не может быть больше даты выхода";
				}
				if (propertyName == string.Empty || propertyName == "EnterDateTime" || propertyName == "ExitDateTime")
				{
					if (EnterDateTime == null || ExitDateTime == null)
						result = "Введите дату";
				}
				CurrentError = result;
				return result;
			}
		}
	}
}
