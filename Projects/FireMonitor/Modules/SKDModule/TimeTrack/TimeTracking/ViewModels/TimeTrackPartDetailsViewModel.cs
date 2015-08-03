using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackPartDetailsViewModel: SaveCancelDialogViewModel
	{
		#region Fields
		readonly DayTimeTrack _dayTimeTrack;
		private readonly TimeTrackDetailsViewModel _parent;
		#endregion

		#region Properties

		public bool IsNeedAdjustment { get; set; }

		private bool _isManuallyAdded;

		public bool IsManuallyAdded
		{
			get { return _isManuallyAdded; }
			set
			{
				_isManuallyAdded = value;
				OnPropertyChanged(() => IsManuallyAdded);
			}
		}

		private bool _notTakeInCalculations;

		public bool NotTakeInCalculations
		{
			get { return _notTakeInCalculations; }
			set
			{
				_notTakeInCalculations = value;
				OnPropertyChanged(() => NotTakeInCalculations);
			}
		}

		TimeSpan _enterTime;
		public TimeSpan EnterTime
		{
			get { return _enterTime; }
			set
			{
				_enterTime = value;
				OnPropertyChanged(() => EnterTime);
			}
		}

		TimeSpan _exitTime;
		public TimeSpan ExitTime
		{
			get { return _exitTime; }
			set
			{
				_exitTime = value;
				OnPropertyChanged(() => ExitTime);
			}
		}

		private DateTime? _enterDateTime;

		public DateTime? EnterDateTime
		{
			get { return _enterDateTime; }
			set
			{
				if (Equals(_enterDateTime, value)) return;
				_enterDateTime = value;
				OnPropertyChanged(() => EnterDateTime);
			}
		}

		private DateTime? _exitDateTime;

		public DateTime? ExitDateTime
		{
			get { return _exitDateTime; }
			set
			{
				if (Equals(_exitDateTime, value)) return;
				_exitDateTime = value;
				OnPropertyChanged(() => ExitDateTime);
			}
		}

		//public DateTime EnterDateTime { get { return _dayTimeTrack.Date.Date.Add(EnterTime); } }
	//	public DateTime ExitDateTime { get { return _dayTimeTrack.Date.Date.Add(ExitTime); } }

		public Guid UID { get; private set; }

		public List<TimeTrackZone> Zones { get; private set; }

		TimeTrackZone _selectedZone;
		public TimeTrackZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		#endregion

		#region Constructors

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, Guid? uid = null, TimeSpan? enterTime = null, TimeSpan? exitTime = null)
		{
			_dayTimeTrack = dayTimeTrack;
			_parent = parent;
			if (uid != null)
			{
				UID = uid.Value;
				EnterTime = enterTime.Value;
				ExitTime = exitTime.Value;
				Title = "Редактировать проход";
			}
			else
			{
				UID = Guid.NewGuid();
				Title = "Добавить проход";
				EnterDateTime = DateTime.Now;

			}

			Zones = new List<TimeTrackZone>(GetMergedZones(employee));
			SelectedZone = Zones.FirstOrDefault();
		}
		#endregion

		private static List<TimeTrackZone> GetMergedZones(ShortEmployee employee)
		{
			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule == null) return SKDManager.Zones.Select(x => new TimeTrackZone(x)).ToList();

			return SKDManager.Zones.Select(zone => schedule.Zones.Any(x => x.ZoneUID == zone.UID)
				? new TimeTrackZone(zone) {IsURV = true}
				: new TimeTrackZone(zone)).ToList();
		}

		#region Commands

		protected override bool Save()
		{
			return Validate();
		}

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}

		#endregion

		#region Methods

		public bool Validate()
		{
			if (_parent == null || !IsIntersection(_parent)) return true;
			MessageBoxService.Show("Невозможно добавить пересекающийся интервал");
			return false;
		}

		public bool IsIntersection(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			return timeTrackDetailsViewModel.DayTimeTrackParts.Any(x => x.UID != UID &&
																		(x.EnterTimeSpan < EnterTime && x.ExitTimeSpan > EnterTime || x.EnterTimeSpan < ExitTime && x.ExitTimeSpan > ExitTime));
		}

		#endregion
	}
}
