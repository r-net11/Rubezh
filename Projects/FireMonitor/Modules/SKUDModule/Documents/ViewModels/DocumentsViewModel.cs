using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : ViewPartViewModel
	{
		public DocumentsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new DocumentFilter();
			Initialize();
		}

		DocumentFilter Filter;

		void Initialize()
		{
			Documents = new ObservableCollection<OrganisationDocumentsViewModel>();
			var documents = FiresecManager.FiresecService.GetDocuments(Filter);

			var dictionary = new Dictionary<Guid, Document>();
			var organisationDocuments = new List<OrganisationDocument>();
			foreach (var document in documents)
			{
				var organisationDocument = organisationDocuments.FirstOrDefault(x => x.OrganisationUID == document.OrganizationUid);
				if (organisationDocument == null)
				{
					organisationDocument = new OrganisationDocument() { OrganisationUID = document.OrganizationUid.Value };
					organisationDocuments.Add(organisationDocument);
				}
				organisationDocument.Documents.Add(document);
			}

			foreach (var organisationDocument in organisationDocuments)
			{
				var documentViewModel = new OrganisationDocumentsViewModel();
				documentViewModel.Initialize(organisationDocument.OrganisationUID.ToString(), organisationDocument.Documents);
				Documents.Add(documentViewModel);
			}
			SelectedDocument = Documents.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<OrganisationDocumentsViewModel> _documents;
		public ObservableCollection<OrganisationDocumentsViewModel> Documents
		{
			get { return _documents; }
			set
			{
				_documents = value;
				OnPropertyChanged("Documents");
			}
		}

		OrganisationDocumentsViewModel _selectedDocument;
		public OrganisationDocumentsViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged("SelectedDocument");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var documentFilterViewModel = new DocumentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(documentFilterViewModel))
			{
				Filter = documentFilterViewModel.Filter;
				Initialize();
			}
		}
	}

	public class OrganisationDocument
	{
		public OrganisationDocument()
		{
			Documents = new List<Document>();
		}

		public Guid OrganisationUID { get; set; }
		public List<Document> Documents { get; set; }
	}
}