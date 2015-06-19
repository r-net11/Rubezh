using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism;

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

		public DateTime EnterDateTime { get { return _DayTimeTrack.Date.Date.Add(EnterTime); } }
		public DateTime ExitDateTime { get { return _DayTimeTrack.Date.Date.Add(ExitTime); } }

		public Guid UID { get; private set; }
		DayTimeTrack _DayTimeTrack;
		private TimeTrackDetailsViewModel _Parent;

		//TODO:Remove this
		public TimeTrackPartDetailsViewModel(TimeTrackDetailsViewModel parent, TimeSpan? enterTime = null, TimeSpan? exitTime = null)
		{
			EnterTime = enterTime.Value;
			ExitTime = exitTime.Value;
			_Parent = parent;
		}

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, Guid? uid = null, TimeSpan? enterTime = null, TimeSpan? exitTime = null)
		{
			_DayTimeTrack = dayTimeTrack;
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
				Title = "Добавить проход";
			}

			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);

			if (schedule == null) return;

			Zones = new ObservableCollection<TimeTrackZone>();

			var strazhZones = SKDManager.Zones.Where(x => schedule.Zones.Any(y => y.ZoneUID == x.UID)).ToList();
			var gkZones = GKManager.SKDZones.Where(x => schedule.Zones.Any(y => y.ZoneUID == x.UID)).ToList();

			Zones.AddRange(
					strazhZones
					.Select(strazhZone => new TimeTrackZone(strazhZone))
					.Union(gkZones.Select(gkZone => new TimeTrackZone(gkZone)))
					.ToList()
				);

			SelectedZone = Zones.FirstOrDefault();
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
			return true;
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}

		public bool Validate()
		{
			if (_Parent == null || !IsIntersection(_Parent)) return true;
			MessageBoxService.Show("Невозможно добавить пересекающийся интервал");
			return false;
		}

		public bool IsIntersection(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			return timeTrackDetailsViewModel.DayTimeTrackParts.Any(x => x.UID != UID &&
																		(x.EnterTimeSpan < EnterTime && x.ExitTimeSpan > EnterTime || x.EnterTimeSpan < ExitTime && x.ExitTimeSpan > ExitTime));
		}
	}

	public class TimeTrackZone
	{
		public SKDZone SKDZone { get; private set; }
		public GKSKDZone GKSKDZone { get; private set; }
		public Guid UID { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public TimeTrackZone(SKDZone zone)
		{
			SKDZone = zone;
			UID = zone.UID;
			No = zone.No;
			Name = zone.Name;
			Description = zone.Description;
		}

		public TimeTrackZone(GKSKDZone zone)
		{
			GKSKDZone = zone;
			UID = zone.UID;
			No = zone.No;
			Name = zone.Name;
			Description = zone.Description;
		}
	}
}
