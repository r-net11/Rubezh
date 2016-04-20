using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using SKDModule.Events;
using System.Collections.ObjectModel;
using System.Linq;

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

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var timeTrackPart in DayTimeTrack.RealTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(timeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}

			Documents = new ObservableCollection<DocumentViewModel>();
			foreach (var document in dayTimeTrack.Documents)
			{
				if (document.EndDateTime > dayTimeTrack.Date.Date)
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
				}
			}
		}


		bool _IsChanged;
		public bool IsChanged
		{
			get { return _IsChanged; }
			set
			{
				_IsChanged = value;
				OnPropertyChanged(() => IsChanged);
			}
		}


		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }

		DayTimeTrackPartViewModel _selectedDayTimeTrackPart;
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


		public RelayCommand AddCustomPartCommand { get; private set; }
		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);
			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				DayTimeTrackParts.Add(new DayTimeTrackPartViewModel(timeTrackPartDetailsViewModel.UID, timeTrackPartDetailsViewModel.EnterTime, timeTrackPartDetailsViewModel.ExitTime, timeTrackPartDetailsViewModel.SelectedZone.Name));
				IsChanged = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}
		bool CanAddPart()
		{
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
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
			return SelectedDayTimeTrackPart != null && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		public RelayCommand EditPartCommand { get; private set; }
		void OnEditPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this, SelectedDayTimeTrackPart.UID, SelectedDayTimeTrackPart.EnterTimeSpan, SelectedDayTimeTrackPart.ExitTimeSpan);
			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				SelectedDayTimeTrackPart.Update(timeTrackPartDetailsViewModel.EnterTime, timeTrackPartDetailsViewModel.ExitTime, timeTrackPartDetailsViewModel.SelectedZone.Name);
				IsChanged = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}


		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeTrackDocument = new TimeTrackDocument();
			timeTrackDocument.StartDateTime = DayTimeTrack.Date.Date;
			timeTrackDocument.EndDateTime = DayTimeTrack.Date.Date;
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, timeTrackDocument);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = ShortEmployee.UID;
				var operationResult = ClientManager.FiresecService.AddTimeTrackDocument(document);
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
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				var operationResult = ClientManager.FiresecService.EditTimeTrackDocument(document);
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
			return SelectedDocument != null && SelectedDocument.Document.StartDateTime.Date == DayTimeTrack.Date.Date && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить документ?"))
			{
				var operationResult = ClientManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
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
			return SelectedDocument != null && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
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

		public bool IsIntersection(TimeTrackPartDetailsViewModel timeTrackPartDetailsViewModel)
		{
			var enterTime = timeTrackPartDetailsViewModel.EnterTime;
			var exitTime = timeTrackPartDetailsViewModel.ExitTime;
			var uid = timeTrackPartDetailsViewModel.UID;
			return DayTimeTrackParts.Any(x => x.UID != uid &&
				(x.EnterTimeSpan <= enterTime && x.ExitTimeSpan > enterTime
				|| x.EnterTimeSpan < exitTime && x.ExitTimeSpan > exitTime));
		}

		protected override bool Save()
		{
			return base.Save();
		}


	}
}