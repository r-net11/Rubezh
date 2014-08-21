using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack)
		{
			dayTimeTrack.Calculate();

			Title = "Время сотрудника в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
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

		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }

		public ObservableCollection<DocumentViewModel> Documents { get; private set; }

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = DayTimeTrack.EmployeeUID;
				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
					SelectedDocument = documentViewModel;
				}
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = DayTimeTrack.EmployeeUID;
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				SelectedDocument.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null && SelectedDocument.Document.StartDateTime.Date == DateTime.Now.Date;
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