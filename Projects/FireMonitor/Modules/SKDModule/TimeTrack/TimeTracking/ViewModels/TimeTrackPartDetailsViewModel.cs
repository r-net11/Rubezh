using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeTrackPartDetailsViewModel: SaveCancelDialogViewModel
	{
		TimeSpan _EnterTime;
		public TimeSpan EnterTime 
		{
			get { return _EnterTime; } 
			set
			{
				_EnterTime = value;
				OnPropertyChanged(() => EnterTime);
			}
		}

		TimeSpan _ExitTime;
		public TimeSpan ExitTime
		{
			get { return _ExitTime; }
			set
			{
				_ExitTime = value;
				OnPropertyChanged(() => ExitTime);
			}
		}

		public Guid UID { get; private set; }
		DayTimeTrack _DayTimeTrack;
		ShortEmployee _Employee;
		bool _IsNew;
		TimeTrackDetailsViewModel _Parent;

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, Guid? uid = null, TimeSpan? enterTime = null, TimeSpan? exitTime = null)
		{
			_DayTimeTrack = dayTimeTrack;
			_Employee = employee;
			_Parent = parent;
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
				_IsNew = true;
				Title = "Добавить проход";
			}

			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule != null)
			{
				var zones = SKDManager.Zones.Where(x => schedule.Zones.Any(y => y.ZoneUID == x.UID));
				Zones = new ObservableCollection<SKDZone>(zones);
				SelectedZone = Zones.FirstOrDefault();
			}
		}

		public ObservableCollection<SKDZone> Zones { get; private set; }

		SKDZone _SelectedZone;
		public SKDZone SelectedZone
		{
			get { return _SelectedZone; }
			set
			{
				_SelectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		protected override bool Save()
		{
			if (!Validate())
				return false;
			var enterDateTime = _DayTimeTrack.Date.Date.Add(EnterTime);
			var exitDateTime = _DayTimeTrack.Date.Date.Add(ExitTime);
			if (_IsNew)
				return PassJournalHelper.AddCustomPassJournal(UID, _Employee.UID, SelectedZone.UID, enterDateTime, exitDateTime);
			else
				return PassJournalHelper.EditPassJournal(UID, SelectedZone.UID, enterDateTime, exitDateTime);
		}

		bool Validate()
		{
			if (EnterTime > ExitTime)
			{
				MessageBoxService.Show("Время входа не может бытьбольше времени выхода");
				return false;
			}
			if (EnterTime == ExitTime)
			{
				MessageBoxService.Show("Невозможно добавить нулевое пребывание в зоне");
				return false;
			}
			if (_Parent.IsIntersection(this))
			{
				MessageBoxService.Show("Невозможно добавить пересекающийся интервал");
				return false;
			}
			return true;				 
		}
	}

}
