using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; set; }

		public TimeTrackViewModel(TimeTrackFilter timeTrackFilter, ShortEmployee shortEmployee, List<DayTimeTrack> dayTimeTracks)
		{
			ShortEmployee = shortEmployee;
			if (dayTimeTracks == null)
				dayTimeTracks = new List<DayTimeTrack>();

			DayTracks = new ObservableCollection<DayTrackViewModel>();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				dayTimeTrack.Calculate();
				var dayTrackViewModel = new DayTrackViewModel(dayTimeTrack, timeTrackFilter);
				DayTracks.Add(dayTrackViewModel);
			}

			Totals = new List<TimeTrackTotal>();
			Totals.Add(new TimeTrackTotal(TimeTrackType.Balance));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Presence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Absence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.AbsenceInsidePlan));
			Totals.Add(new TimeTrackTotal(TimeTrackType.PresenceInBrerak));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Late));
			Totals.Add(new TimeTrackTotal(TimeTrackType.EarlyLeave));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Overtime));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Night));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DayOff));
			Totals.Add(new TimeTrackTotal(TimeTrackType.Holiday));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentOvertime));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentPresence));
			Totals.Add(new TimeTrackTotal(TimeTrackType.DocumentAbsence));

			foreach (var dayTimeTrack in dayTimeTracks)
			{
				foreach (var timeTrackTotal in dayTimeTrack.Totals)
				{
					var total = Totals.FirstOrDefault(x => x.TimeTrackType == timeTrackTotal.TimeTrackType);
					if (total != null)
					{
						total.TimeSpan += timeTrackTotal.TimeSpan;
					}
				}
			}
			OnPropertyChanged(() => Totals);
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }
		public List<TimeTrackTotal> Totals { get; private set; }
	}
}