using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : ViewPartViewModel
	{
		DocumentFilter Filter;

		public DocumentsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new DocumentFilter();
			Initialize();
		}

		void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var documents = DocumentHelper.Get(Filter);

			OrganisationDocuments = new ObservableCollection<OrganisationDocumentsViewModel>();
			foreach (var organisation in organisations)
			{
				var documentViewModel = new OrganisationDocumentsViewModel();
				documentViewModel.Initialize(organisation, new List<Document>(documents.Where(x => x.OrganizationUID.HasValue && x.OrganizationUID.Value == organisation.UID)));
				OrganisationDocuments.Add(documentViewModel);
			}
			SelectedOrganisationDocument = OrganisationDocuments.FirstOrDefault();
		}

		ObservableCollection<OrganisationDocumentsViewModel> _organisationDocuments;
		public ObservableCollection<OrganisationDocumentsViewModel> OrganisationDocuments
		{
			get { return _organisationDocuments; }
			set
			{
				_organisationDocuments = value;
				OnPropertyChanged("OrganisationDocuments");
			}
		}

		OrganisationDocumentsViewModel _selectedOrganisationDocument;
		public OrganisationDocumentsViewModel SelectedOrganisationDocument
		{
			get { return _selectedOrganisationDocument; }
			set
			{
				_selectedOrganisationDocument = value;
				OnPropertyChanged("SelectedOrganisationDocument");
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

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}