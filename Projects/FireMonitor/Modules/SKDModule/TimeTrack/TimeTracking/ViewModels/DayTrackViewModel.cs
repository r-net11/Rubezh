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
			foreach (var totalTimeTrackTypeFilter in TimeTrackFilter.TotalTimeTrackTypeFilters)
			{
				var timeTrackTotal = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == totalTimeTrackTypeFilter);
				if (timeTrackTotal != null)
				{
					Totals.Add(new TotalViewModel(timeTrackTotal.TimeTrackType, timeTrackTotal.TimeSpan));
				}
			}
			OnPropertyChanged(() => Totals);
		}

		public string Letter { get; private set; }
		public string Tooltip { get; private set; }
		public List<TotalViewModel> Totals { get; private set; }
	}
}