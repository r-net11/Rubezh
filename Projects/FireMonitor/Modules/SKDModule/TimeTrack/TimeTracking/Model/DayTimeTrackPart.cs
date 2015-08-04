using System.Globalization;
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

		public Guid UID { get; private set; }

		private bool _isManuallyAdded;

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

		private DateTime? _exitDateTime;

		public DateTime? ExitDateTime
		{
			get { return _exitDateTime; }
			set { this.RaiseAndSetIfChanged(ref _exitDateTime, value); }
		}

		TimeSpan _enterTimeSpan;
		public TimeSpan EnterTimeSpan
		{
			get { return _enterTimeSpan; }
			set { this.RaiseAndSetIfChanged(ref _enterTimeSpan, value); }
		}

		TimeSpan _exitTimeSpan;
		public TimeSpan ExitTimeSpan
		{
			get { return _exitTimeSpan; }
			set { this.RaiseAndSetIfChanged(ref _exitTimeSpan, value); }
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

		public string CorrectedDate { get; set; }

		public string CorrectedBy { get; set; }

		private TimeTrackZone _timeTrackZone;

		public TimeTrackZone TimeTrackZone
		{
			get { return _timeTrackZone; }
			set
			{
				_timeTrackZone = value;
				this.RaiseAndSetIfChanged(ref _timeTrackZone, value);
			}
		}

		#endregion

		#region Constructors

		public DayTimeTrackPart(TimeTrackPartDetailsViewModel timeTrackPartDetailsViewModel)
		{
			UID = timeTrackPartDetailsViewModel.UID;
			CorrectedBy = FiresecManager.CurrentUser.Name;
			CorrectedDate = DateTime.Now.ToString(CultureInfo.CurrentUICulture);
			Update(
				timeTrackPartDetailsViewModel.EnterDateTime,
				timeTrackPartDetailsViewModel.EnterDateTime,
				timeTrackPartDetailsViewModel.EnterTime,
				timeTrackPartDetailsViewModel.ExitTime,
				timeTrackPartDetailsViewModel.SelectedZone,
				timeTrackPartDetailsViewModel.NotTakeInCalculations,
				timeTrackPartDetailsViewModel.IsManuallyAdded);
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart, ShortEmployee employee)
		{
			var zone =
				TimeTrackingHelper.GetMergedZones(employee).FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID)
				??
				new TimeTrackZone { Name = "<Нет в конфигурации>", No = default(int) };

			UID = timeTrackPart.PassJournalUID;
			Update(
				timeTrackPart.EnterDateTime,
				timeTrackPart.ExitDateTime,
				timeTrackPart.StartTime,
				timeTrackPart.EndTime,
				zone,
				timeTrackPart.NotTakeInCalculations,
				timeTrackPart.IsManuallyAdded);
		}

		public DayTimeTrackPart()
		{
		}

		#endregion

		#region Methods

		public void Update(DateTime? enterDateTime, DateTime? exitDateTime, TimeSpan enterTime, TimeSpan exitTime, TimeTrackZone timeTrackZone, bool notTakeInCalculations, bool isManuallyAdded)
		{
			TimeTrackZone = timeTrackZone;
			EnterDateTime = enterDateTime;
			ExitDateTime = exitDateTime;
			EnterTimeSpan = enterTime;
			ExitTimeSpan = exitTime;
			NotTakeInCalculations = notTakeInCalculations;
			IsManuallyAdded = isManuallyAdded;
		}

		#endregion

		public string this[string propertyName]
		{
			get
			{
				var result = string.Empty;
				propertyName = propertyName ?? string.Empty;
				if (propertyName == string.Empty || propertyName == "EnterTime")
				{
					if (EnterTimeSpan > ExitTimeSpan)
						result = "Время входа не может быть больше времени выхода";
				}
				if (propertyName == string.Empty || propertyName == "ExitTime")
				{
					if (EnterTimeSpan == ExitTimeSpan)
						result = "Невозможно добавить нулевое пребывание в зоне";
				}
				return result;
			}
		}
	}
}
