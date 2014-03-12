﻿using System.Collections.ObjectModel;
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
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var documents = DocumentHelper.GetDocuments(Filter);

			OrganisationDocuments = new ObservableCollection<OrganisationDocumentsViewModel>();
			foreach (var organisation in organisations)
			{
				var documentViewModel = new OrganisationDocumentsViewModel();
				documentViewModel.Initialize(organisation.Name, new List<Document>(documents.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationDocuments.Add(documentViewModel);
			}
			SelectedOrganisationDocument = OrganisationDocuments.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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
	}
}