using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using SKDModule.Model;
using SKDModule.Helpers;

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

		private bool _isEnabledTakeInCalculations;

		public bool IsEnabledTakeInCalculations
		{
			get { return _isEnabledTakeInCalculations; }
			set
			{
				if (_isEnabledTakeInCalculations == value) return;
				_isEnabledTakeInCalculations = value;
				OnPropertyChanged(() => IsEnabledTakeInCalculations);
			}
		}

		#endregion

		#region Constructors

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, Guid? uid = null, DateTime? enterDateTime = null, DateTime? exitDateTime = null)
		{
			_dayTimeTrack = dayTimeTrack;
			_parent = parent;
			if (uid != null)
			{
				UID = uid.Value;
				EnterDateTime = enterDateTime.HasValue ? enterDateTime.Value.Date : dayTimeTrack.Date;
				EnterTime = enterDateTime.HasValue ? enterDateTime.Value.TimeOfDay : dayTimeTrack.Date.TimeOfDay;
				ExitDateTime = exitDateTime.HasValue ? exitDateTime.Value.Date : dayTimeTrack.Date.Date;
				ExitTime = exitDateTime.HasValue ? exitDateTime.Value.TimeOfDay : dayTimeTrack.Date.TimeOfDay;
				Title = "Редактировать проход";
			}
			else
			{
				UID = Guid.NewGuid();
				Title = "Добавить проход";
				EnterDateTime = dayTimeTrack.Date;
				ExitDateTime = dayTimeTrack.Date;
			}

			Zones = new List<TimeTrackZone>(TimeTrackingHelper.GetMergedZones(employee));
			SelectedZone = Zones.FirstOrDefault();
			this.WhenAny(x => x.SelectedZone, x => x.Value)
				.Subscribe(value =>
				{
					if (value != null && !value.IsURV)
					{
						IsEnabledTakeInCalculations = false;
						NotTakeInCalculations = true;
					}
					else
					{
						IsEnabledTakeInCalculations = true;
						NotTakeInCalculations = false;
					}
				});
		}
		#endregion

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
			if (EnterDateTime == null || ExitDateTime == null)
			{
				MessageBoxService.Show("Выберите дату начала и дату конца интервала");
				return false;
			}

			if (_parent == null || !IsIntersection(_parent)) return true;
			MessageBoxService.Show("Невозможно добавить пересекающийся интервал");
			return false;
		}

		public bool IsIntersection(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			return timeTrackDetailsViewModel.DayTimeTrackParts.Any(x => x.UID != UID &&
																		(x.EnterDateTime < (EnterDateTime + EnterTime) &&
																		x.ExitDateTime > (EnterDateTime + EnterTime) ||
																		x.EnterDateTime < (ExitDateTime + ExitTime) &&
																		x.ExitDateTime > (ExitDateTime + ExitTime)));
		}

		#endregion
	}
}
