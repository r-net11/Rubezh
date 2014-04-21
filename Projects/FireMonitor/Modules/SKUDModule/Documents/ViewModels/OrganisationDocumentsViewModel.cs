using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using SKDModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationDocumentsViewModel : BaseViewModel
	{
		public Organisation Organization { get; private set; }

		public OrganisationDocumentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(Organisation organization, List<Document> documents)
		{
			Organization = organization;

			Documents = new ObservableCollection<DocumentViewModel>();
			foreach (var document in documents)
			{
				var documentViewModel = new DocumentViewModel(document);
				Documents.Add(documentViewModel);
			}
			SelectedDocument = Documents.FirstOrDefault();
		}

		ObservableCollection<DocumentViewModel> _documents;
		public ObservableCollection<DocumentViewModel> Documents
		{
			get { return _documents; }
			set
			{
				_documents = value;
				OnPropertyChanged("Documents");
			}
		}

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged("SelectedDocument");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.Document;
				var saveResult = DocumentHelper.Save(document);
				if (!saveResult)
					return;
				var documentViewModel = new DocumentViewModel(document);
				Documents.Add(documentViewModel);
				SelectedDocument = documentViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var document = SelectedDocument.Document;
			var removeResult = DocumentHelper.MarkDeleted(document);
			if (!removeResult)
				return;
			var index = Documents.IndexOf(SelectedDocument);
			Documents.Remove(SelectedDocument);
			index = Math.Min(index, Documents.Count - 1);
			if (index > -1)
				SelectedDocument = Documents[index];
		}
		bool CanRemove()
		{
			return SelectedDocument != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.Document;
				var saveResult = DocumentHelper.Save(document);
				if (!saveResult)
					return;
				SelectedDocument.Update(document);
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null;
		}
	}
}