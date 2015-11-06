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
		private IEnumerable<TimeTrackDocumentType> SystemDocumentTypes { get; set; }

		public DocumentTypesViewModel(IEnumerable<TimeTrackDocumentType> systemDocumentTypes)
		{
			Title = "Документы";
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			SystemDocumentTypes = systemDocumentTypes;

			Organisations = new List<DocumentType>();
			Initialize(systemDocumentTypes);

			OnPropertyChanged(() => Organisations);
			SelectedDocumentType = Organisations.FirstOrDefault();
		}

		private void Initialize(IEnumerable<TimeTrackDocumentType> systemDocumentTypes)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations != null)
			{
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
			}

			var systemOrganisation = new Organisation
			{
				Name = "Документы по умолчанию"
			};

			var systemOrganisationViewModel = new DocumentType(systemOrganisation);
			foreach (var document in systemDocumentTypes)
			{
				var documentTypeViewModel = new DocumentType(systemOrganisation, document, true);
				systemOrganisationViewModel.AddChild(documentTypeViewModel);
			}

			Organisations.Add(systemOrganisationViewModel);
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
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType, SystemDocumentTypes);
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
		bool CanAdd()
		{
			return SelectedDocumentType != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentTypeDetailsViewModel = new DocumentTypeDetailsViewModel(SelectedDocumentType, SystemDocumentTypes, true);
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
			if (!MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить тип документа?")) return;

			var organisationViewModel = SelectedDocumentType;
			if (!organisationViewModel.IsOrganisation)
				organisationViewModel = SelectedDocumentType.Parent;

			if (organisationViewModel == null || organisationViewModel.Organisation == null)
				return;

			var timeTrackDocumentType = SelectedDocumentType.TimeTrackDocumentType;
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
			return SelectedDocumentType != null && !SelectedDocumentType.IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);
		}
	}
}