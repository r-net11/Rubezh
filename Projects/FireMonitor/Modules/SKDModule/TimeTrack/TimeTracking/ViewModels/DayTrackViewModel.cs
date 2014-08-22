using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class DayTrackViewModel : BaseViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }
		public TimeTrackFilter TimeTrackFilter { get; private set; }

		public DayTrackViewModel(DayTimeTrack dayTimeTrack, TimeTrackFilter timeTrackFilter)
		{
			DayTimeTrack = dayTimeTrack;
			TimeTrackFilter = timeTrackFilter;
			Update();
		}

		public void Update()
		{
			Letter = DayTimeTrack.LetterCode;
			Tooltip = DayTimeTrack.TimeTrackType.ToDescription();

			if (DayTimeTrack.Documents.Count > 0)
			{
				var timeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == DayTimeTrack.Documents[0].DocumentCode);
				if (timeTrackDocumentType != null)
				{
					Tooltip = timeTrackDocumentType.Name;
				}
			}

			OnPropertyChanged(() => Letter);
			OnPropertyChanged(() => Tooltip);
			OnPropertyChanged(() => DayTimeTrack);

			Totals = new List<TotalViewModel>();
			if (TimeTrackFilter.IsTotal)
				Totals.Add(new TotalViewModel("Присутствие", DayTimeTrack.Total));
			if (TimeTrackFilter.IsTotalMissed)
				Totals.Add(new TotalViewModel("Пропущено", DayTimeTrack.TotalMissed));
			if (TimeTrackFilter.IsTotalInSchedule)
				Totals.Add(new TotalViewModel("По графику", DayTimeTrack.TotalInSchedule));
			if (TimeTrackFilter.IsTotalOvertime)
				Totals.Add(new TotalViewModel("Переработка", DayTimeTrack.TotalOvertime));
			if (TimeTrackFilter.IsTotalLate)
				Totals.Add(new TotalViewModel("Опоздания", DayTimeTrack.TotalLate));
			if (TimeTrackFilter.IsTotalEarlyLeave)
				Totals.Add(new TotalViewModel("Ранний уход", DayTimeTrack.TotalEarlyLeave));
			if (TimeTrackFilter.IsTotalPlanned)
				Totals.Add(new TotalViewModel("По графику", DayTimeTrack.TotalPlanned));
			if (TimeTrackFilter.IsTotalEavening)
				Totals.Add(new TotalViewModel("В вечерние часы", DayTimeTrack.TotalEavening));
			if (TimeTrackFilter.IsTotalNight)
				Totals.Add(new TotalViewModel("В ночные часы", DayTimeTrack.TotalNight));
			if (TimeTrackFilter.IsTotal_DocumentOvertime)
				Totals.Add(new TotalViewModel("Переработка по документу", DayTimeTrack.Total_DocumentOvertime));
			if (TimeTrackFilter.IsTotal_DocumentPresence)
				Totals.Add(new TotalViewModel("Присутствие по документу", DayTimeTrack.Total_DocumentPresence));
			if (TimeTrackFilter.IsTotal_DocumentAbsence)
				Totals.Add(new TotalViewModel("Отсутствие по документу", DayTimeTrack.Total_DocumentAbsence));
			OnPropertyChanged(() => Totals);
		}

		public string Letter { get; private set; }
		public string Tooltip { get; private set; }
		public List<TotalViewModel> Totals { get; private set; }
	}
}