﻿using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
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
			Organisations = new List<DocumentType>();
			Initialize();

			OnPropertyChanged(() => Organisations);
			SelectedDocumentType = Organisations.FirstOrDefault();
		}

		private void Initialize()
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					var organisationViewModel = new DocumentType(organisation);

					Organisations.Add(organisationViewModel);

					var documentsForOrganisation = DocumentTypeHelper.GetByOrganisation(organisation.UID).ToList();

					foreach (var documentType in documentsForOrganisation)
					{
						if (documentType.OrganisationUID == organisation.UID)
						{
							var documentTypeViewModel = new DocumentType(organisation, documentType);

							organisationViewModel.AddChild(documentTypeViewModel);
						}
					}
				}
			}
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
				var organisationViewModel = SelectedDocumentType;
				if (!organisationViewModel.IsOrganisation)
					organisationViewModel = SelectedDocumentType.Parent;

				if (organisationViewModel.Organisation != null)
					return organisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType, DocumentsForSelectedOrganisation());
			if (DialogService.ShowModalWindow(documentTypeDetailsViewModel))
			{
				if (DocumentTypeHelper.Add(documentTypeDetailsViewModel.TimeTrackDocumentType))
				{
					var documentViewModel = new DocumentType(SelectedDocumentType.Organisation, documentTypeDetailsViewModel.TimeTrackDocumentType);

					var organisationViewModel = SelectedDocumentType;
					if (!organisationViewModel.IsOrganisation)
						organisationViewModel = SelectedDocumentType.Parent;

					if (organisationViewModel == null || organisationViewModel.Organisation == null)
						return;

					organisationViewModel.AddChild(documentViewModel);
					SelectedDocumentType = documentViewModel;
				}
			}
		}

		private IEnumerable<TimeTrackDocumentType> DocumentsForSelectedOrganisation()
		{
			IEnumerable<DocumentType> documentsForSelectedOrganisation;

			if (SelectedDocumentType.IsOrganisation)
			{
				documentsForSelectedOrganisation = SelectedDocumentType.GetAllChildren().Where(x => !x.IsOrganisation);
			}
			else
			{
				var testOrganisation = SelectedDocumentType.GetAllParents().FirstOrDefault();
				documentsForSelectedOrganisation = testOrganisation != null ? testOrganisation.GetAllChildren().Where(x => !x.IsOrganisation) : null;
			}

			return documentsForSelectedOrganisation != null
				? documentsForSelectedOrganisation.Select(x => x.TimeTrackDocumentType)
				: null;
		}

		bool CanAdd()
		{
			return SelectedDocumentType != null
				&& FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			//TODO: DocumentsForSelectedOrganisation() can be replaced by SelectedDocumentType.Organisation
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType, DocumentsForSelectedOrganisation(), true);
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
			return SelectedDocumentType != null
				&& SelectedDocumentType.Parent != null
				&& !SelectedDocumentType.IsOrganisation
				&& FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (!MessageBoxService.ShowQuestion(Resources.RemoveDocumentTypeMessage)) return;

			var organisationViewModel = SelectedDocumentType;
			if (!organisationViewModel.IsOrganisation)
				organisationViewModel = SelectedDocumentType.Parent;

			if (organisationViewModel == null || organisationViewModel.Organisation == null)
				return;

			var timeTrackDocumentType = SelectedDocumentType.TimeTrackDocumentType;
			if (!DocumentTypeHelper.CheckDocumentType(timeTrackDocumentType, SelectedDocumentType.Organisation.UID))
			{
				MessageBoxService.ShowWarning(
					string.Format("Документы вида \"{0}\" введены для некоторых сотрудников. Для удаления вида оправдательных документов необходимо предварительно удалить эти документы или изменить их вид", SelectedDocumentType.Name)
					);
				return;
			}
			var removeResult = DocumentTypeHelper.Remove(timeTrackDocumentType.UID);
			if (!removeResult)
				return;

			var index = organisationViewModel.Children.ToList().IndexOf(SelectedDocumentType);
			organisationViewModel.RemoveChild(SelectedDocumentType);
			index = Math.Min(index, organisationViewModel.Children.Count() - 1);
			SelectedDocumentType = index > -1 ? organisationViewModel.Children.ToList()[index] : organisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedDocumentType != null
				&& !SelectedDocumentType.IsOrganisation
				&& SelectedDocumentType.TimeTrackDocumentType != null
				&& !SelectedDocumentType.TimeTrackDocumentType.IsSystem
				&& FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}
	}
}