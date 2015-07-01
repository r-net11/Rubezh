using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }
		public ShortEmployee ShortEmployee { get; private set; }

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee shortEmployee)
		{
			if(string.IsNullOrEmpty(dayTimeTrack.Error))
				dayTimeTrack.Calculate();

			Title = "Время сотрудника " + shortEmployee.FIO + " в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			AddFileCommand = new RelayCommand(OnAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile);
			AddCustomPartCommand = new RelayCommand(OnAddCustomPart, CanAddPart);
			RemovePartCommand = new RelayCommand(OnRemovePart, CanEditRemovePart);
			EditPartCommand = new RelayCommand(OnEditPart, CanEditRemovePart);
			DayTimeTrack = dayTimeTrack;
			ShortEmployee = shortEmployee;

			DayTimeTrackParts = GetCollection(DayTimeTrack.RealTimeTrackParts);

			Documents = GetCollection(DayTimeTrack.Documents);

			this.WhenAny(x => x.SelectedDocument, x => x.Value)
				.Subscribe(value =>
				{
					CanDoChanges = value != null && value.HasFile;
				});
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


		bool _isDirty;
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				_isDirty = value;
				OnPropertyChanged(() => IsDirty);
			}
		}

		private bool _canDoChanges;
		public bool CanDoChanges
		{
			get { return _canDoChanges; }
			set
			{
				if (_canDoChanges == value) return;
				_canDoChanges = value;
				OnPropertyChanged(() => CanDoChanges);
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
		/// <summary>
		/// Функция создания прохода
		/// </summary>
		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);
			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				DayTimeTrackParts.Add(new DayTimeTrackPartViewModel(timeTrackPartDetailsViewModel));
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel;
				DayTimeTrack.Date.Date.Add(timeTrackPartDetailsViewModel.ExitTime);

				IsDirty = true;
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
				IsDirty = true;
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
				IsDirty = true;
				IsNew = default(bool);
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeTrackDocument = new TimeTrackDocument
			{
				StartDateTime = DayTimeTrack.Date.Date,
				EndDateTime = DayTimeTrack.Date.Date + new TimeSpan(23, 59, 59)
			};

			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, timeTrackDocument);

			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				documentDetailsViewModel.TimeTrackDocument.EmployeeUID = ShortEmployee.UID;

				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					var documentViewModel = new DocumentViewModel(documentDetailsViewModel.TimeTrackDocument);
					Documents.Add(documentViewModel);
					SelectedDocument = documentViewModel;
					IsDirty = true;
					ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(documentDetailsViewModel.TimeTrackDocument);
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
				IsDirty = true;
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
					IsDirty = true;
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

		public RelayCommand OpenFileCommand { get; private set; }
		void OnOpenFile()
		{
			SelectedDocument.OpenFile();
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			SelectedDocument.RemoveFile();
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