using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
			EditCommand = new RelayCommand(OnEdit, CanEditOrRemoveDoc);
			RemoveCommand = new RelayCommand(OnRemove, CanEditOrRemoveDoc);
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile, CanEditOrRemove);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanEditOrRemove);

			Documents = new ObservableCollection<TimeTrackAttachedDocument>();
			if (timeTrackEmployeeResult.Documents != null)
			{
				foreach (var document in timeTrackEmployeeResult.Documents.OrderBy(x => x.StartDateTime))
				{
					var documentViewModel = new TimeTrackAttachedDocument(document);
					Documents.Add(documentViewModel);
				}
			}
			SelectedDocument = Documents.FirstOrDefault();
			IsDirty = false;
				//this.ObservableForProperty(x => x.SelectedDocument)
				//	.Value()
				//	.Where(value => value.HasFile)
				//	.ToProperty(this, x => x.CanDoChanges);

			//this.WhenAny(x => x.SelectedDocument, x => x.Value).Select(x => x != null)
			//var isSearchEnabled = this.ObservableForProperty(x => x.SelectedDocument)
			//.Select(x => x.Value != null);
			//this.AddFileCommand =
			//	new ReactiveAsyncCommand(Observable.Return<bool>(false));
			//AddFileCommand.RegisterAsyncAction(_ => SelectedDocument.AddFile());
			//.Subscribe(x => this.WhenAny(doc => doc.SelectedDocument, doc => doc.Value).Select(doc => doc.HasFile));
			//AddFileCommand.Subscribe(this.WhenAny(x => x.SelectedDocument, x => x.Value).Subscribe(value =>
			//{
			//	value != null;
			//})
			//	)
			//this.AddFileCommand = ReactiveAsyncCommand.Create(
			//   pauseOrContinueCommand.Select(x => x.CanExecuteObservable).Switch().ObserveOn(RxApp.MainThreadScheduler),
			//   async _ =>
			//   {
			//	   IReactiveCommand<Unit> command = await pauseOrContinueCommand.FirstAsync();
			//	   await command.ExecuteAsync();
			//   });
		}

		public ObservableCollection<TimeTrackAttachedDocument> Documents { get; private set; }

		TimeTrackAttachedDocument _selectedDocument;
		public TimeTrackAttachedDocument SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(true, OrganisationUID, EmployeeUID);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					var documentViewModel = new TimeTrackAttachedDocument(documentDetailsViewModel.TimeTrackDocument);
					Documents.Add(documentViewModel);
					SelectedDocument = documentViewModel;
					IsDirty = true;
				}
			}
		}
		private bool CanAdd()
		{
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		private bool CanEditOrRemove()
		{
			return SelectedDocument != null
				&& SelectedDocument.HasFile
				&& FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(true, OrganisationUID, EmployeeUID,
				SelectedDocument.Document)
			{
				SelectedDocumentType = SelectedDocument.Document.TimeTrackDocumentType.DocumentType
			};

			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				SelectedDocument.Update();
				IsDirty = true;
			}
		}
		private bool CanEditOrRemoveDoc()
		{
			return SelectedDocument != null && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(CommonViewModels.DeleteDocument))
			{
				var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					Documents.Remove(SelectedDocument);
					IsDirty = true;
				}
			}
		}

		public RelayCommand AddFileCommand { get; private set; }
		private void OnAddFile()
		{
			SelectedDocument.AddFile();
		}

		public RelayCommand OpenFileCommand { get; private set; }
		void OnOpenFile()
		{
			SelectedDocument.OpenFile();
		}

		private bool CanAddFile()
		{
			return SelectedDocument != null && SelectedDocument.Document != null;
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			SelectedDocument.RemoveFile();
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
					Documents.Add(new TimeTrackAttachedDocument(document));
				}
				IsDirty = true;
			}
		}

		public void OnRemoveDocument(TimeTrackDocument document)
		{
			var viewModel = Documents.FirstOrDefault(x => x.Document.UID == document.UID);
			if (viewModel != null)
			{
				Documents.Remove(viewModel);
				OnPropertyChanged(() => Documents);
				IsDirty = true;
			}
		}

		public void OnEditTimeTrackPart(Guid uid)
		{
			if (EmployeeUID == uid)
			{
				IsDirty = true;
			}
		}
		#endregion
	}
}