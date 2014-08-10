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
			Title = "Время в течение дня";
			DayTimeTrack = dayTimeTrack;

			AvailableExceptionTypes = new ObservableCollection<TimeTrackExceptionType>(Enum.GetValues(typeof(TimeTrackExceptionType)).OfType<TimeTrackExceptionType>());

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var timeTrackPart in DayTimeTrack.RealTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(timeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}

			if (DayTimeTrack.TimeTrackException != null)
			{
				SelectedExceptionType = AvailableExceptionTypes.FirstOrDefault(x => x == DayTimeTrack.TimeTrackException.TimeTrackExceptionType);
				Comment = DayTimeTrack.TimeTrackException.Comment;
			}
			hasChanges = false;
		}

		public ObservableCollection<TimeTrackExceptionType> AvailableExceptionTypes { get; private set; }

		TimeTrackExceptionType _selectedExcuseDocument;
		public TimeTrackExceptionType SelectedExceptionType
		{
			get { return _selectedExcuseDocument; }
			set
			{
				_selectedExcuseDocument = value;
				OnPropertyChanged(() => SelectedExceptionType);
				IsCommentEnabled = value != TimeTrackExceptionType.None;
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
			if (DayTimeTrack.TimeTrackException == null)
			{
				DayTimeTrack.TimeTrackException = new TimeTrackException();
			}
			DayTimeTrack.TimeTrackException.EmployeeUID = DayTimeTrack.EmployeeUID;
			DayTimeTrack.TimeTrackException.StartDateTime = DayTimeTrack.Date.Date;
			DayTimeTrack.TimeTrackException.EndDateTime = DayTimeTrack.Date.Date;
			DayTimeTrack.TimeTrackException.TimeTrackExceptionType = SelectedExceptionType;
			DayTimeTrack.TimeTrackException.Comment = Comment;

			if (hasChanges)
			{
				var operationResult = FiresecManager.FiresecService.SaveTimeTrackException(DayTimeTrack.TimeTrackException);
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