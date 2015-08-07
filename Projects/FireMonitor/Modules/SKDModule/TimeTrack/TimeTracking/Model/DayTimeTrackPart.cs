using System.Globalization;
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

		public bool IsEnabledNotTakeInCalculations
		{
			get
			{
				if (TimeTrackZone.IsURV) return true;
				NotTakeInCalculations = true;
				return false;
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

		private DateTime? _exitDateTime;

		public DateTime? ExitDateTime
		{
			get { return _exitDateTime; }
			set { this.RaiseAndSetIfChanged(ref _exitDateTime, value); }
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
			set
			{
				if (string.Equals(_correctedDate, value)) return;
				_correctedDate = value;
				this.RaiseAndSetIfChanged(ref _correctedDate, value);

			}
		}

		private string _correctedBy;

		public string CorrectedBy
		{
			get { return _correctedBy; }
			set
			{
				if (string.Equals(_correctedBy, value)) return;
				_correctedBy = value;
				this.RaiseAndSetIfChanged(ref _correctedBy, value);
			}
		}

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
			Update(
				timeTrackPartDetailsViewModel.EnterDateTime + timeTrackPartDetailsViewModel.EnterTime,
				timeTrackPartDetailsViewModel.EnterDateTime + timeTrackPartDetailsViewModel.ExitTime,
				timeTrackPartDetailsViewModel.SelectedZone,
				timeTrackPartDetailsViewModel.NotTakeInCalculations,
				timeTrackPartDetailsViewModel.IsManuallyAdded,
				DateTime.Now.ToString(CultureInfo.CurrentUICulture),
				FiresecManager.CurrentUser.Name);
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart, ShortEmployee employee)
		{
			var zone =
				TimeTrackingHelper.GetMergedZones(employee).FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID)
				??
				new TimeTrackZone { Name = "<Нет в конфигурации>", No = default(int) };

			UID = timeTrackPart.PassJournalUID;
			var user = timeTrackPart.IsManuallyAdded
						?
						(FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.UID == timeTrackPart.CorrectedByUID)
						??
						new User {Name = "<Нет в конфигурации>"})
						:
						new User{Name = string.Empty};

			Update(
				timeTrackPart.EnterDateTime,
				timeTrackPart.ExitDateTime,
				zone,
				timeTrackPart.NotTakeInCalculations,
				timeTrackPart.IsManuallyAdded,
				timeTrackPart.AdjustmentDate.ToString(),
				user.Name);
		}

		public DayTimeTrackPart()
		{
		}

		#endregion

		#region Methods

		public void Update(DateTime? enterDateTime, DateTime? exitDateTime,
			TimeTrackZone timeTrackZone, bool notTakeInCalculations, bool isManuallyAdded, string adjustmentDate, string correctedBy)
		{
			TimeTrackZone = timeTrackZone;
			EnterDateTime = enterDateTime;
			ExitDateTime = exitDateTime;
			NotTakeInCalculations = notTakeInCalculations;
			IsManuallyAdded = isManuallyAdded;
			CorrectedDate = adjustmentDate;
			CorrectedBy = correctedBy;
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
					if (EnterDateTime > ExitDateTime)
						result = "Время входа не может быть больше времени выхода";
				}
				if (propertyName == string.Empty || propertyName == "ExitTime")
				{
					if (EnterDateTime == ExitDateTime)
						result = "Невозможно добавить нулевое пребывание в зоне";
				}
				return result;
			}
		}
	}
}
