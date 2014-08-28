using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using FiresecAPI.SKD;
using SKDModule.Common;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class DocumentTypesViewModel : DialogViewModel
	{
		TimeTrackDocumentType _clipboard;

		public DocumentTypesViewModel()
		{
			Title = "Документы";
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);

			Organisations = new List<DocumentTypeViewModel>();
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;

			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DocumentTypeViewModel(organisation);
				Organisations.Add(organisationViewModel);

				var documentTypes = DocumentTypeHelper.GetByOrganisation(organisation.UID);
				foreach (var documentType in documentTypes)
				{
					if (documentType.OrganisationUID == organisation.UID)
					{
						var documentTypeViewModel = new DocumentTypeViewModel(organisation, documentType);
						organisationViewModel.AddChild(documentTypeViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedDocumentType = Organisations.FirstOrDefault();
		}

		public List<DocumentTypeViewModel> Organisations { get; private set; }

		DocumentTypeViewModel _selectedDocumentType;
		public DocumentTypeViewModel SelectedDocumentType
		{
			get { return _selectedDocumentType; }
			set
			{
				_selectedDocumentType = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedDocumentType);
			}
		}

		public DocumentTypeViewModel ParentOrganisation
		{
			get
			{
				DocumentTypeViewModel OrganisationViewModel = SelectedDocumentType;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDocumentType.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType.Organisation.UID);
			if (DialogService.ShowModalWindow(documentTypeDetailsViewModel))
			{
				if (DocumentTypeHelper.Add(documentTypeDetailsViewModel.TimeTrackDocumentType))
				{
					var documentViewModel = new DocumentTypeViewModel(SelectedDocumentType.Organisation, documentTypeDetailsViewModel.TimeTrackDocumentType);

					DocumentTypeViewModel OrganisationViewModel = SelectedDocumentType;
					if (!OrganisationViewModel.IsOrganisation)
						OrganisationViewModel = SelectedDocumentType.Parent;

					if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
						return;

					OrganisationViewModel.AddChild(documentViewModel);
					SelectedDocumentType = documentViewModel;
				}
			}
		}
		bool CanAdd()
		{
			return SelectedDocumentType != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType.Organisation.UID, SelectedDocumentType.TimeTrackDocumentType);
			if (DialogService.ShowModalWindow(documentTypeDetailsViewModel))
			{
				if (DocumentTypeHelper.Edit(documentTypeDetailsViewModel.TimeTrackDocumentType))
				{
					SelectedDocumentType.Update(documentTypeDetailsViewModel.TimeTrackDocumentType);
				}
			}
		}
		bool CanEdit()
		{
			return SelectedDocumentType != null && SelectedDocumentType.Parent != null && !SelectedDocumentType.IsOrganisation;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			DocumentTypeViewModel OrganisationViewModel = SelectedDocumentType;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedDocumentType.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var timeTrackDocumentType = SelectedDocumentType.TimeTrackDocumentType;
			bool removeResult = DocumentTypeHelper.Remove(timeTrackDocumentType.UID);
			if (!removeResult)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedDocumentType);
			OrganisationViewModel.RemoveChild(SelectedDocumentType);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedDocumentType = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedDocumentType = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedDocumentType != null && !SelectedDocumentType.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_clipboard = CopyDocumentType(SelectedDocumentType.TimeTrackDocumentType, false);
		}
		bool CanCopy()
		{
			return SelectedDocumentType != null && !SelectedDocumentType.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			if (ParentOrganisation != null)
			{
				var newDocumentType = CopyDocumentType(_clipboard);
				newDocumentType.UID = Guid.NewGuid();
				var timeTrackDocumentType = new TimeTrackDocumentType()
				{
					UID = newDocumentType.UID,
					Name = newDocumentType.Name,
					ShortName = newDocumentType.ShortName,
					OrganisationUID = newDocumentType.OrganisationUID,
				};
				if (DocumentTypeHelper.Add(timeTrackDocumentType))
				{
					var documentTypeViewModel = new DocumentTypeViewModel(SelectedDocumentType.Organisation, newDocumentType);

					ParentOrganisation.AddChild(documentTypeViewModel);
					SelectedDocumentType = documentTypeViewModel;
				}
			}
		}
		bool CanPaste()
		{
			return SelectedDocumentType != null && _clipboard != null;
		}

		TimeTrackDocumentType CopyDocumentType(TimeTrackDocumentType source, bool newName = true)
		{
			var copy = new TimeTrackDocumentType();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.ShortName = source.ShortName;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}