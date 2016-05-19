using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : BaseViewModel
	{
		public Guid EmployeeUID { get; private set; }
		public Guid OrganisationUID { get; private set; }

		public DocumentsViewModel(TimeTrackEmployeeResult timeTrackEmployeeResult, DateTime startDate, DateTime endDate)
		{
			EmployeeUID = timeTrackEmployeeResult.ShortEmployee.UID;
			OrganisationUID = timeTrackEmployeeResult.ShortEmployee.OrganisationUID;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);

			Documents = new ObservableCollection<DocumentViewModel>();
			if (timeTrackEmployeeResult.Documents != null)
			{
				foreach (var document in timeTrackEmployeeResult.Documents.OrderBy(x => x.StartDateTime))
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
				}
			}
			SelectedDocument = Documents.FirstOrDefault();
			IsChanged = false;
			
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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(true, OrganisationUID);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = EmployeeUID;
				var operationResult = ClientManager.RubezhService.AddTimeTrackDocument(document);
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
			var documentDetailsViewModel = new DocumentDetailsViewModel(true, OrganisationUID, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = EmployeeUID;
				var operationResult = ClientManager.RubezhService.EditTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				SelectedDocument.Update();
				IsChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить документ?"))
			{
				var operationResult = ClientManager.RubezhService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
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

		#region ForEvent
		public void OnEditDocument(TimeTrackDocument document)
		{
			if (document.EmployeeUID == EmployeeUID)
			{
				var viewModel = Documents.FirstOrDefault(x => x.Document.UID == document.UID);
				if (viewModel != null)
				{
					viewModel.Update(document);
				}
				else
				{
					Documents.Add(new DocumentViewModel(document));
				}
				IsChanged = true;
			}
		}

		public void OnRemoveDocument(TimeTrackDocument document)
		{
			var viewModel = Documents.FirstOrDefault(x => x.Document.UID == document.UID);
			if (viewModel != null)
			{
				Documents.Remove(viewModel);
				OnPropertyChanged(() => Documents);
				IsChanged = true;
			}
		}

		public void OnEditTimeTrackPart(Guid uid)
		{
			if (EmployeeUID == uid)
			{
				IsChanged = true;
			}
		}
		#endregion
	}
}