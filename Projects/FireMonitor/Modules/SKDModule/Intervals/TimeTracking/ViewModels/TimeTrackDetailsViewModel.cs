using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack)
		{
			Title = "Время в течение дня";
			DayTimeTrack = dayTimeTrack;

			AvailableExcuseDocuments = new ObservableCollection<ExcuseDocumentEnum>(Enum.GetValues(typeof(ExcuseDocumentEnum)).OfType<ExcuseDocumentEnum>());

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var timeTrackPart in DayTimeTrack.RealTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(timeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}
		}

		public ObservableCollection<ExcuseDocumentEnum> AvailableExcuseDocuments { get; private set; }

		ExcuseDocumentEnum _selectedExcuseDocument;
		public ExcuseDocumentEnum SelectedExcuseDocument
		{
			get { return _selectedExcuseDocument; }
			set
			{
				_selectedExcuseDocument = value;
				OnPropertyChanged(() => SelectedExcuseDocument);
			}
		}

		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }
	}

	public class DayTimeTrackPartViewModel : BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public string EnterTime { get; private set; }
		public string ExitTime { get; private set; }

		public DayTimeTrackPartViewModel(TimeTrackPart timeTrackPart)
		{
			Zone = SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
			EnterTime = timeTrackPart.StartTime.Hours.ToString("00") + ":" + timeTrackPart.StartTime.Minutes.ToString("00") + ":" + timeTrackPart.StartTime.Seconds.ToString("00");
			ExitTime = timeTrackPart.EndTime.Hours.ToString("00") + ":" + timeTrackPart.EndTime.Minutes.ToString("00") + ":" + timeTrackPart.EndTime.Seconds.ToString("00");
		}
	}
}