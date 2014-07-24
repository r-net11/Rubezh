using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public DocumentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(DocumentFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			var documents = DocumentHelper.Get(filter);

			AllDocuments = new List<DocumentViewModel>();
			Organisations = new List<DocumentViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DocumentViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllDocuments.Add(organisationViewModel);
				foreach (var document in documents)
				{
					if (document.OrganisationUID == organisation.UID)
					{
						var documentViewModel = new DocumentViewModel(organisation, document);
						organisationViewModel.AddChild(documentViewModel);
						AllDocuments.Add(documentViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedDocument = Organisations.FirstOrDefault();
		}

		public List<DocumentViewModel> Organisations { get; private set; }
		List<DocumentViewModel> AllDocuments { get; set; }

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

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(SelectedDocument.Organisation);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var documentViewModel = new DocumentViewModel(SelectedDocument.Organisation, documentDetailsViewModel.ShortDocument);

				DocumentViewModel OrganisationViewModel = SelectedDocument;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDocument.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
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

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedDocument);
			var document = SelectedDocument.Document;
			bool removeResult = DocumentHelper.MarkDeleted(document.UID);
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
			var documentDetailsViewModel = new DocumentDetailsViewModel(SelectedDocument.Organisation, SelectedDocument.Document.UID);
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