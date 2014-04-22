using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		DocumentFilter Filter;

		public DocumentsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Filter = new DocumentFilter() { OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var documents = DocumentHelper.Get(Filter);

			AllDocuments = new List<DocumentViewModel>();
			Organisations = new List<DocumentViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DocumentViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllDocuments.Add(organisationViewModel);
				foreach (var document in documents)
				{
					if (document.OrganizationUID == organisation.UID)
					{
						var documentViewModel = new DocumentViewModel(document);
						organisationViewModel.AddChild(documentViewModel);
						AllDocuments.Add(documentViewModel);
					}
				}
			}

			foreach (var organisation in Organisations)
			{
				organisation.ExpandToThis();
			}
			OnPropertyChanged("RootDocuments");
		}

		#region DocumentSelection
		public List<DocumentViewModel> AllDocuments;

		public void Select(Guid documentUID)
		{
			if (documentUID != Guid.Empty)
			{
				var documentViewModel = AllDocuments.FirstOrDefault(x => x.Document != null && x.Document.UID == documentUID);
				if (documentViewModel != null)
					documentViewModel.ExpandToThis();
				SelectedDocument = documentViewModel;
			}
		}
		#endregion

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedDocument");
			}
		}

		List<DocumentViewModel> _organisations;
		public List<DocumentViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public Organisation Organisation
		{
			get
			{
				DocumentViewModel OrganisationViewModel = SelectedDocument;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDocument.Parent;

				if (OrganisationViewModel != null)
					return OrganisationViewModel.Organization;

				return null;
			}
		}

		public DocumentViewModel[] RootDocuments
		{
			get { return Organisations.ToArray(); }
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new DocumentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this, Organisation);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var documentViewModel = new DocumentViewModel(documentDetailsViewModel.ShortDocument);

				DocumentViewModel OrganisationViewModel = SelectedDocument;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDocument.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organization == null)
					return;

				OrganisationViewModel.AddChild(documentViewModel);
				SelectedDocument = documentViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedDocument != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			DocumentViewModel OrganisationViewModel = SelectedDocument;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedDocument.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organization == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedDocument);
			var document = SelectedDocument.Document;
			bool removeResult = DocumentHelper.MarkDeleted(document);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedDocument);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedDocument = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedDocument = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedDocument != null && !SelectedDocument.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this, Organisation, SelectedDocument.Document.UID);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				SelectedDocument.Update(documentDetailsViewModel.ShortDocument);
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null && SelectedDocument.Parent != null && !SelectedDocument.IsOrganisation;
		}
	}
}