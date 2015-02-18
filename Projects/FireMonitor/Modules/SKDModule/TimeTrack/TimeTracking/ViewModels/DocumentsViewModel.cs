using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

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
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
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
			var documentDetailsViewModel = new DocumentDetailsViewModel(true, OrganisationUID, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = EmployeeUID;
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(document);
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
			return SelectedDocument != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
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
					Documents.Remove(SelectedDocument);
					IsChanged = true;
				}
			}
		}
		bool CanRemove()
		{
			return SelectedDocument != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnEditDocument(TimeTrackDocument document)
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

		void OnRemoveDocument(TimeTrackDocument document)
		{
			var viewModel = Documents.FirstOrDefault(x => x.Document.UID == document.UID);
			if (viewModel != null)
			{
				Documents.Remove(viewModel);
				OnPropertyChanged(() => Documents);
				IsChanged = true;
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
	}
}