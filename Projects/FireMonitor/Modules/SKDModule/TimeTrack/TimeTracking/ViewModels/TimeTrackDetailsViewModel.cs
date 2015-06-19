using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }
		public ShortEmployee ShortEmployee { get; private set; }

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee shortEmployee)
		{
			dayTimeTrack.Calculate();

			Title = "Время сотрудника " + shortEmployee.FIO + " в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);
			AddCustomPartCommand = new RelayCommand(OnAddCustomPart, CanAddPart);
			RemovePartCommand = new RelayCommand(OnRemovePart, CanEditRemovePart);
			EditPartCommand = new RelayCommand(OnEditPart, CanEditRemovePart);
			DayTimeTrack = dayTimeTrack;
			ShortEmployee = shortEmployee;

			DayTimeTrackParts = GetCollection(DayTimeTrack.RealTimeTrackParts);

			Documents = GetCollection(DayTimeTrack.Documents);
		}

		private ObservableCollection<DocumentViewModel> GetCollection(IEnumerable<TimeTrackDocument> timeTrackDocuments)
		{
			var result = new ObservableCollection<DocumentViewModel>();

			foreach (var document in timeTrackDocuments)
			{
				result.Add(new DocumentViewModel(document));
			}

			return result;
		}

		private ObservableCollection<DayTimeTrackPartViewModel> GetCollection(IEnumerable<TimeTrackPart> timeTrackParts)
		{
			var result = new ObservableCollection<DayTimeTrackPartViewModel>();

			foreach (var timeTrackPart in timeTrackParts)
			{
				result.Add(new DayTimeTrackPartViewModel(timeTrackPart));
			}

			return result;
		}


		bool _isChanged;
		public bool IsChanged
		{
			get { return _isChanged; }
			set
			{
				_isChanged = value;
				OnPropertyChanged(() => IsChanged);
			}
		}

		public bool IsNew { get; set; }

		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }

		DayTimeTrackPartViewModel _selectedDayTimeTrackPart; //TODO: remove it and save all DayTimeTrackParts
		public DayTimeTrackPartViewModel SelectedDayTimeTrackPart
		{
			get { return _selectedDayTimeTrackPart; }
			set
			{
				_selectedDayTimeTrackPart = value;
				OnPropertyChanged(() => SelectedDayTimeTrackPart);
			}
		}

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

		private TimeTrackPartDetailsViewModel _selectedTimeTrackPartDetailsViewModel;

		public TimeTrackPartDetailsViewModel SelectedTimeTrackPartDetailsViewModel
		{
			get { return _selectedTimeTrackPartDetailsViewModel; }
			set
			{
				if (_selectedTimeTrackPartDetailsViewModel == value) return;
				_selectedTimeTrackPartDetailsViewModel = value;
				OnPropertyChanged(() => SelectedTimeTrackPartDetailsViewModel);
			}
		}


		public RelayCommand AddCustomPartCommand { get; private set; }
		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);
			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				DayTimeTrackParts.Add(new DayTimeTrackPartViewModel(timeTrackPartDetailsViewModel));
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel;
					DayTimeTrack.Date.Date.Add(timeTrackPartDetailsViewModel.ExitTime);

				IsChanged = true;
				IsNew = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}
		bool CanAddPart()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		public RelayCommand RemovePartCommand { get; private set; }
		void OnRemovePart()
		{
			var result = PassJournalHelper.DeleteAllPassJournalItems(SelectedDayTimeTrackPart.UID, DayTimeTrack.Date + SelectedDayTimeTrackPart.EnterTimeSpan, DayTimeTrack.Date + SelectedDayTimeTrackPart.ExitTimeSpan);
			if (result)
			{
				DayTimeTrackParts.Remove(SelectedDayTimeTrackPart);
				SelectedDayTimeTrackPart = DayTimeTrackParts.FirstOrDefault();
				IsChanged = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}
		bool CanEditRemovePart()
		{
			return SelectedDayTimeTrackPart != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		public RelayCommand EditPartCommand { get; private set; }
		void OnEditPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this, SelectedDayTimeTrackPart.UID, SelectedDayTimeTrackPart.EnterTimeSpan, SelectedDayTimeTrackPart.ExitTimeSpan);
			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel;
				SelectedDayTimeTrackPart.Update(timeTrackPartDetailsViewModel.EnterTime, timeTrackPartDetailsViewModel.ExitTime, timeTrackPartDetailsViewModel.SelectedZone.Name, timeTrackPartDetailsViewModel.SelectedZone.No);
				IsChanged = true;
				IsNew = default(bool);
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeTrackDocument = new TimeTrackDocument();
			timeTrackDocument.StartDateTime = DayTimeTrack.Date.Date;
			timeTrackDocument.EndDateTime = DayTimeTrack.Date.Date + new TimeSpan(23, 59, 59);
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, timeTrackDocument);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = ShortEmployee.UID;
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
					IsChanged = true;
					ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(document);
				}
			}
		}
		bool CanAdd()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(document);
				SelectedDocument.Update();
				IsChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null && SelectedDocument.Document.StartDateTime.Date == DayTimeTrack.Date.Date && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить документ?"))
			{
				var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Publish(SelectedDocument.Document);
					Documents.Remove(SelectedDocument);
					IsChanged = true;
				}
			}
		}
		bool CanRemove()
		{
			return SelectedDocument != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand AddFileCommand { get; private set; }
		void OnAddFile()
		{
			SelectedDocument.AddFile();
		}
		bool CanAddFile()
		{
			return SelectedDocument != null && !SelectedDocument.HasFile;
		}

		public RelayCommand OpenFileCommand { get; private set; }
		void OnOpenFile()
		{
			SelectedDocument.OpenFile();
		}
		bool CanOpenFile()
		{
			return SelectedDocument != null && SelectedDocument.HasFile;
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			SelectedDocument.RemoveFile();
		}
		bool CanRemoveFile()
		{
			return SelectedDocument != null && SelectedDocument.HasFile;
		}

		protected override bool CanSave()
		{
			return (DayTimeTrackParts.Any(x => !x.IsValid) == false) && SelectedTimeTrackPartDetailsViewModel != null;
		}

		protected override bool Save() //TODO: Save all TimeTracks without refresh
		{
			if (IsNew)
				PassJournalHelper.AddCustomPassJournal(Guid.NewGuid(), ShortEmployee.UID,
					SelectedTimeTrackPartDetailsViewModel.SelectedZone.UID,
					DayTimeTrack.Date.Date.Add(SelectedTimeTrackPartDetailsViewModel.EnterTime),
					DayTimeTrack.Date.Date.Add(SelectedTimeTrackPartDetailsViewModel.ExitTime));
			else
				PassJournalHelper.EditPassJournal(SelectedDayTimeTrackPart.UID,
					SelectedTimeTrackPartDetailsViewModel.SelectedZone.UID,
					DayTimeTrack.Date.Date.Add(SelectedTimeTrackPartDetailsViewModel.EnterTime),
					DayTimeTrack.Date.Date.Add(SelectedTimeTrackPartDetailsViewModel.ExitTime));

			return base.Save();
		}
	}
}