using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayTrackViewModel : BaseViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }

		public DayTrackViewModel(DayTimeTrack dayTimeTrack)
		{
			DayTimeTrack = dayTimeTrack;
			Update();
		}

		public void Update()
		{
			Total = DateTimeToString(DayTimeTrack.Total);
			TotalMissed = DateTimeToString(DayTimeTrack.TotalMissed);
			TotalInSchedule = DateTimeToString(DayTimeTrack.TotalInSchedule);
			TotalOutSchedule = DateTimeToString(DayTimeTrack.TotalOutSchedule);

			IsNormal = false;
			HasLetter = false;
			Letter = null;

			switch (DayTimeTrack.TimeTrackType)
			{
				case TimeTrackType.Holiday:
					HasLetter = true;
					Letter = "В";
					break;

				case TimeTrackType.Missed:
					HasLetter = true;
					Letter = "ПР";
					break;

				case TimeTrackType.DayOff:
					HasLetter = true;
					Letter = "В";
					break;

				default:
					IsNormal = true;
					break;
			}
			Tooltip = DayTimeTrack.TimeTrackType.ToDescription();

			if (DayTimeTrack.TimeTrackDocument != null && DayTimeTrack.TimeTrackDocument.DocumentCode != 0)
			{
				IsNormal = false;
				HasLetter = true;
				var timeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == DayTimeTrack.TimeTrackDocument.DocumentCode);
				if (timeTrackDocumentType != null)
				{
					Letter = timeTrackDocumentType.ShortName;
					Tooltip = timeTrackDocumentType.Name;
				}
			}

			OnPropertyChanged(() => IsNormal);
			OnPropertyChanged(() => HasLetter);
			OnPropertyChanged(() => Letter);
			OnPropertyChanged(() => Tooltip);
			OnPropertyChanged(() => DayTimeTrack);
		}

		public string Total { get; private set; }
		public string TotalMissed { get; private set; }
		public string TotalInSchedule { get; private set; }
		public string TotalOutSchedule { get; private set; }

		public bool IsNormal { get; private set; }
		public bool HasLetter { get; private set; }
		public string Letter { get; private set; }
		public string Tooltip { get; private set; }

		public static string DateTimeToString(TimeSpan timeSpan)
		{
			if (timeSpan.TotalHours > 0)
				return ((int)timeSpan.TotalHours).ToString() + "ч " + ((int)timeSpan.Minutes).ToString() + "мин";
			else if (timeSpan.Minutes > 0)
				return ((int)timeSpan.Minutes).ToString() + "мин";
			return null;
		}
	}
}