using System.Globalization;
using FiresecAPI.SKD;
using FiresecClient;
using ReactiveUI;
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
		public int No { get; private set; }

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

		string _zoneName;
		public string ZoneName
		{
			get { return _zoneName; }
			set { this.RaiseAndSetIfChanged(ref _zoneName, value); }
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
				timeTrackPartDetailsViewModel.SelectedZone.Name,
				timeTrackPartDetailsViewModel.SelectedZone.No,
				timeTrackPartDetailsViewModel.NotTakeInCalculations,
				timeTrackPartDetailsViewModel.IsManuallyAdded);
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart)
		{
			string zoneName = null;
			var num = default(int);

			var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
			if (strazhZone != null)
			{
				zoneName = strazhZone.Name;
				num = strazhZone.No;
			}

			UID = timeTrackPart.PassJournalUID;
			Update(
				timeTrackPart.EnterDateTime,
				timeTrackPart.ExitDateTime,
				timeTrackPart.StartTime,
				timeTrackPart.EndTime,
				zoneName,
				num,
				timeTrackPart.NotTakeInCalculations,
				timeTrackPart.IsManuallyAdded);
		}

		public DayTimeTrackPart()
		{
		}

		#endregion

		#region Methods

		public void Update(DateTime? enterDateTime, DateTime? exitDateTime, TimeSpan enterTime, TimeSpan exitTime, string zoneName, int no, bool notTakeInCalculations, bool isManuallyAdded)
		{
			ZoneName = zoneName ?? "<Нет в конфигурации>";
			No = no;
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
