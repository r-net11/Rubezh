using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack)
		{
			dayTimeTrack.Calculate();

			AddCommand = new RelayCommand(OnAdd);
			Title = "Время сотрудника в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			DayTimeTrack = dayTimeTrack;

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var timeTrackPart in DayTimeTrack.RealTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(timeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}

			Documents = new ObservableCollection<DocumentViewModel>();
			if (dayTimeTrack.Documents != null)
			{
				foreach (var document in dayTimeTrack.Documents)
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
				}
			}
		}

		public ObservableCollection<DocumentViewModel> Documents { get; private set; }
		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}

	public class DayTimeTrackPartViewModel : BaseViewModel
	{
		public string ZoneName { get; private set; }
		public string EnterTime { get; private set; }
		public string ExitTime { get; private set; }

		public DayTimeTrackPartViewModel(TimeTrackPart timeTrackPart)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
			if (zone != null)
			{
				ZoneName = zone.Name;
			}
			else
			{
				ZoneName = "<Нет в конфигурации>";
			}
			EnterTime = timeTrackPart.StartTime.Hours.ToString("00") + ":" + timeTrackPart.StartTime.Minutes.ToString("00") + ":" + timeTrackPart.StartTime.Seconds.ToString("00");
			ExitTime = timeTrackPart.EndTime.Hours.ToString("00") + ":" + timeTrackPart.EndTime.Minutes.ToString("00") + ":" + timeTrackPart.EndTime.Seconds.ToString("00");
		}
	}
}