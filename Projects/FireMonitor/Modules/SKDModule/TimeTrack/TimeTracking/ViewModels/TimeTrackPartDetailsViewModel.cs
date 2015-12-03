using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class TimeTrackPartDetailsViewModel : SaveCancelDialogViewModel
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

		public DateTime EnterDateTime { get { return _DayTimeTrack.Date.Date.Add(EnterTime); } }
		public DateTime ExitDateTime { get { return _DayTimeTrack.Date.Date.Add(ExitTime); } }

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
				Zones = new ObservableCollection<TimeTrackZone>();

				var gkZones = GKManager.SKDZones.Where(x => schedule.Zones.Any(y => y.ZoneUID == x.UID));
				foreach (var zone in gkZones)
				{
					Zones.Add(new TimeTrackZone(zone));
				}

				SelectedZone = Zones.FirstOrDefault();
			}
		}

		public ObservableCollection<TimeTrackZone> Zones { get; private set; }

		TimeTrackZone _SelectedZone;
		public TimeTrackZone SelectedZone
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
			if (_IsNew)
				return PassJournalHelper.AddCustomPassJournal(UID, _Employee.UID, SelectedZone.UID, EnterDateTime, ExitDateTime);
			else
				return PassJournalHelper.EditPassJournal(UID, SelectedZone.UID, EnterDateTime, ExitDateTime);
		}
		protected override bool CanSave()
		{
			return base.CanSave() && SelectedZone != null;
		}

		bool Validate()
		{
			if (EnterTime > ExitTime)
			{
				MessageBoxService.Show("Время входа не может быть больше времени выхода");
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

	public class TimeTrackZone
	{
		public GKSKDZone GKSKDZone { get; private set; }
		public Guid UID { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public TimeTrackZone(GKSKDZone zone)
		{
			GKSKDZone = zone;
			UID = zone.UID;
			Name = zone.PresentationName;
			Description = zone.Description;
		}
	}
}