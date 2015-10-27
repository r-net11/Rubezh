using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using DocumentType = SKDModule.Model.DocumentType;

namespace SKDModule.ViewModels
{
	public class DocumentTypesViewModel : DialogViewModel
	{
		public DocumentTypesViewModel()
		{
			Title = "Документы";
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			var docFactory = new DocumentsFactory();
			Organisations = new List<DocumentType>();
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;

			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DocumentType(organisation);
				Organisations.Add(organisationViewModel);

				var documentTypes = DocumentTypeHelper.GetByOrganisation(organisation.UID);
				foreach (var documentType in documentTypes)
				{
					if (documentType.OrganisationUID == organisation.UID)
					{
						var documentTypeViewModel = new DocumentType(organisation, documentType);
						organisationViewModel.AddChild(documentTypeViewModel);
					}
				}
			}

			var systemOrganisation = new Organisation
			{
				Name = "Документы по умолчанию"
			};

			var systemOrganisationViewModel = new DocumentType(systemOrganisation);
			foreach (var document in docFactory.SystemDocuments)
			{
				var documentTypeViewModel = new DocumentType(systemOrganisation, document.TimeTrackDocumentType, true);
				systemOrganisationViewModel.AddChild(documentTypeViewModel);
			}

			Organisations.Add(systemOrganisationViewModel);

			OnPropertyChanged(() => Organisations);
			SelectedDocumentType = Organisations.FirstOrDefault();
		}

		public List<DocumentType> Organisations { get; private set; }

		DocumentType _selectedDocumentType;
		public DocumentType SelectedDocumentType
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

		public DocumentType ParentOrganisation
		{
			get
			{
				DocumentType OrganisationViewModel = SelectedDocumentType;
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
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType);
			if (DialogService.ShowModalWindow(documentTypeDetailsViewModel))
			{
				if (DocumentTypeHelper.Add(documentTypeDetailsViewModel.TimeTrackDocumentType))
				{
					var documentViewModel = new DocumentType(SelectedDocumentType.Organisation, documentTypeDetailsViewModel.TimeTrackDocumentType);

					DocumentType OrganisationViewModel = SelectedDocumentType;
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
			return SelectedDocumentType != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType, true);
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
			return SelectedDocumentType != null && SelectedDocumentType.Parent != null && !SelectedDocumentType.IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить тип документа?"))
			{
				DocumentType OrganisationViewModel = SelectedDocumentType;
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
		}
		bool CanRemove()
		{
			return SelectedDocumentType != null && !SelectedDocumentType.IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}
	}
}