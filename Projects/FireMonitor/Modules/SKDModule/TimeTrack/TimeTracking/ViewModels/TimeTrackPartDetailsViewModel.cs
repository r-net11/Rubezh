using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
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

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, Guid? uid = null, TimeSpan? enterTime = null, TimeSpan? exitTime = null)
		{
			_DayTimeTrack = dayTimeTrack;
			_Employee = employee;
			if (uid != null)
			{
				UID = uid.Value;
				EnterTime = enterTime.Value;
				ExitTime = exitTime.Value;
			}
			else
			{
				UID = Guid.NewGuid();
				_IsNew = true;
			}

			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule != null)
			{
				var zones = SKDManager.Zones.Where(x => schedule.Zones.Any(y => y.ZoneUID == x.UID));
				Zones = new ObservableCollection<SKDZone>(zones);
			}
			SelectedZone = Zones.FirstOrDefault();
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
			var enterDateTime = _DayTimeTrack.Date.Date.Add(EnterTime);
			var exitDateTime = _DayTimeTrack.Date.Date.Add(ExitTime);
			if(_IsNew)
				return PassJournalHelper.AddCustomPassJournal(UID, _Employee.UID, SelectedZone.UID, enterDateTime, exitDateTime);
			else
				return PassJournalHelper.EditPassJournal(UID, SelectedZone.UID, enterDateTime, exitDateTime);
		}
	}

}
