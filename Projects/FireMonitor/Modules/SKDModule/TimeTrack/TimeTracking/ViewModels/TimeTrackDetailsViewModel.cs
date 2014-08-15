using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }
		bool hasChanges = false;

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack)
		{
			dayTimeTrack.Calculate();

			Title = "Время в течение дня";
			DayTimeTrack = dayTimeTrack;

			AvailableDocuments = new ObservableCollection<TimeTrackDocumentType>();
			foreach (var timeTrackDocumentType in TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes)
			{
				AvailableDocuments.Add(timeTrackDocumentType);
			}

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var timeTrackPart in DayTimeTrack.RealTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(timeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}

			if (DayTimeTrack.TimeTrackDocument != null)
			{
				SelectedDocument = AvailableDocuments.FirstOrDefault(x => x.Code == DayTimeTrack.TimeTrackDocument.DocumentCode);
				Comment = DayTimeTrack.TimeTrackDocument.Comment;
			}
			hasChanges = false;
		}

		public ObservableCollection<TimeTrackDocumentType> AvailableDocuments { get; private set; }

		TimeTrackDocumentType _selectedDocument;
		public TimeTrackDocumentType SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
				IsCommentEnabled = value != null && value.Code != 0;
				OnPropertyChanged(() => IsCommentEnabled);
				hasChanges = true;
			}
		}

		string _comment;
		public string Comment
		{
			get { return _comment; }
			set
			{
				_comment = value;
				OnPropertyChanged(() => Comment);
				hasChanges = true;
			}
		}

		public bool IsCommentEnabled { get; private set; }

		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }

		protected override bool Save()
		{
			if (DayTimeTrack.TimeTrackDocument == null)
			{
				DayTimeTrack.TimeTrackDocument = new TimeTrackDocument();
			}
			DayTimeTrack.TimeTrackDocument.EmployeeUID = DayTimeTrack.EmployeeUID;
			DayTimeTrack.TimeTrackDocument.StartDateTime = DayTimeTrack.Date.Date;
			DayTimeTrack.TimeTrackDocument.EndDateTime = DayTimeTrack.Date.Date;
			DayTimeTrack.TimeTrackDocument.DocumentCode = SelectedDocument.Code;
			DayTimeTrack.TimeTrackDocument.Comment = Comment;

			if (hasChanges)
			{
				var operationResult = FiresecManager.FiresecService.SaveTimeTrackDocument(DayTimeTrack.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
			}
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