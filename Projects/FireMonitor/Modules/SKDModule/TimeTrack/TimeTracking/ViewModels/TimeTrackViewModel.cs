using System;
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

			var total = new TimeSpan();
			var totalMissed = new TimeSpan();
			var totalInSchedule = new TimeSpan();
			var totalOvertime = new TimeSpan();
			var totalLate = new TimeSpan();
			var totalEarlyLeave = new TimeSpan();
			var totalPlanned = new TimeSpan();
			var totalEavening = new TimeSpan();
			var totalNight = new TimeSpan();
			var total_DocumentOvertime = new TimeSpan();
			var total_DocumentPresence = new TimeSpan();
			var total_DocumentAbsence = new TimeSpan();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				total = total.Add(dayTimeTrack.Total);
				totalMissed = totalMissed.Add(dayTimeTrack.TotalMissed);
				totalInSchedule = totalInSchedule.Add(dayTimeTrack.TotalInSchedule);
				totalOvertime = totalOvertime.Add(dayTimeTrack.TotalOvertime);
				totalLate = totalLate.Add(dayTimeTrack.TotalLate);
				totalEarlyLeave = totalEarlyLeave.Add(dayTimeTrack.TotalEarlyLeave);
				totalPlanned = totalPlanned.Add(dayTimeTrack.TotalPlanned);
				totalEavening = totalEavening.Add(dayTimeTrack.TotalEavening);
				totalNight = totalNight.Add(dayTimeTrack.TotalNight);
				total_DocumentOvertime = total_DocumentOvertime.Add(dayTimeTrack.Total_DocumentOvertime);
				total_DocumentPresence = total_DocumentPresence.Add(dayTimeTrack.Total_DocumentPresence);
				total_DocumentAbsence = total_DocumentAbsence.Add(dayTimeTrack.Total_DocumentAbsence);
			}

			Totals = new List<TotalViewModel>();
			if (timeTrackFilter.IsTotal)
				Totals.Add(new TotalViewModel("Присутствие", total));
			if (timeTrackFilter.IsTotalMissed)
				Totals.Add(new TotalViewModel("Пропущено", totalMissed));
			if (timeTrackFilter.IsTotalInSchedule)
				Totals.Add(new TotalViewModel("По графику", totalInSchedule));
			if (timeTrackFilter.IsTotalOvertime)
				Totals.Add(new TotalViewModel("Переработка", totalOvertime));
			if (timeTrackFilter.IsTotalLate)
				Totals.Add(new TotalViewModel("Опоздания", totalLate));
			if (timeTrackFilter.IsTotalEarlyLeave)
				Totals.Add(new TotalViewModel("Ранний уход", totalEarlyLeave));
			if (timeTrackFilter.IsTotalPlanned)
				Totals.Add(new TotalViewModel("По графику", totalPlanned));
			if (timeTrackFilter.IsTotalEavening)
				Totals.Add(new TotalViewModel("В вечерние часы", totalEavening));
			if (timeTrackFilter.IsTotalNight)
				Totals.Add(new TotalViewModel("В ночные часы", totalNight));
			if (timeTrackFilter.IsTotal_DocumentOvertime)
				Totals.Add(new TotalViewModel("Переработка по документу", total_DocumentOvertime));
			if (timeTrackFilter.IsTotal_DocumentPresence)
				Totals.Add(new TotalViewModel("Присутствие по документу", total_DocumentPresence));
			if (timeTrackFilter.IsTotal_DocumentAbsence)
				Totals.Add(new TotalViewModel("Отсутствие по документу", total_DocumentAbsence));
			OnPropertyChanged(() => Totals);
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }

		public List<TotalViewModel> Totals { get; private set; }
	}
}