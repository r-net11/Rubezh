using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : ViewPartViewModel
	{
		AccessTemplate _clipboard;

		public AccessTemplatesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
		}

		public void Initialize(AccessTemplateFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var accessTemplates = AccessTemplateHelper.Get(filter);
			if (accessTemplates == null)
				return;
			AllAccessTemplates = new ObservableCollection<AccessTemplateViewModel>();
			Organisations = new ObservableCollection<AccessTemplateViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new AccessTemplateViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllAccessTemplates.Add(organisationViewModel);
				if (accessTemplates != null)
				{
					foreach (var accessTemplate in accessTemplates)
					{
						if (accessTemplate.OrganisationUID == organisation.UID)
						{
							var accessTemplateViewModel = new AccessTemplateViewModel(organisation, accessTemplate);
							organisationViewModel.AddChild(accessTemplateViewModel);
							AllAccessTemplates.Add(accessTemplateViewModel);
						}
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedAccessTemplate = Organisations.FirstOrDefault();
		}

		
		void OnEditOrganisation(Organisation newOrganisation)
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
			if (organisation != null)
			{
				organisation.Update(newOrganisation);
			}
			OnPropertyChanged(() => Organisations);
		}

		void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
			{
				var organisationViewModel = new AccessTemplateViewModel(newOrganisation);
				Organisations.Add(organisationViewModel);
				AllAccessTemplates.Add(organisationViewModel);
				var employees = AccessTemplateHelper.GetByOrganisation(newOrganisation.UID);
				if (employees == null)
					return;
				foreach (var additionalColumnType in employees)
				{
					var employeeViewModel = new AccessTemplateViewModel(newOrganisation, additionalColumnType);
					organisationViewModel.AddChild(employeeViewModel);
					AllAccessTemplates.Add(employeeViewModel);
				}
				OnPropertyChanged(() => Organisations);
			}
			else
			{
				var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
				if (organisationViewModel != null)
				{
					Organisations.Remove(organisationViewModel);
					AllAccessTemplates.Remove(organisationViewModel);
					OnPropertyChanged(() => Organisations);
				}
			}
		}

		public ObservableCollection<AccessTemplateViewModel> Organisations { get; private set; }
		ObservableCollection<AccessTemplateViewModel> AllAccessTemplates { get; set; }

		AccessTemplateViewModel _selectedAccessTemplate;
		public AccessTemplateViewModel SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedAccessTemplate);
			}
		}

		public AccessTemplateViewModel ParentOrganisation
		{
			get
			{
				AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAccessTemplate.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(SelectedAccessTemplate.Organisation);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				var accessTemplateViewModel = new AccessTemplateViewModel(SelectedAccessTemplate.Organisation, accessTemplateDetailsViewModel.AccessTemplate);

				AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAccessTemplate.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(accessTemplateViewModel);
				SelectedAccessTemplate = accessTemplateViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedAccessTemplate != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedAccessTemplate.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedAccessTemplate);
			var accessTemplate = SelectedAccessTemplate.AccessTemplate;
			bool removeResult = AccessTemplateHelper.MarkDeleted(accessTemplate);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedAccessTemplate);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedAccessTemplate = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedAccessTemplate = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedAccessTemplate != null && !SelectedAccessTemplate.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(SelectedAccessTemplate.Organisation, SelectedAccessTemplate.AccessTemplate);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				SelectedAccessTemplate.Update(accessTemplateDetailsViewModel.AccessTemplate);
			}
		}
		bool CanEdit()
		{
			return SelectedAccessTemplate != null && SelectedAccessTemplate.Parent != null && !SelectedAccessTemplate.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_clipboard = CopyAccessTemplate(SelectedAccessTemplate.AccessTemplate, false);
		}
		bool CanCopy()
		{
			return SelectedAccessTemplate != null && !SelectedAccessTemplate.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			if (ParentOrganisation != null)
			{
				var newAccessTemplate = CopyAccessTemplate(_clipboard);
				newAccessTemplate.CardDoors.ForEach(x => x.AccessTemplateUID = newAccessTemplate.UID);
				if (AccessTemplateHelper.Save(newAccessTemplate))
				{
					var accessTemplateViewModel = new AccessTemplateViewModel(SelectedAccessTemplate.Organisation, newAccessTemplate);
					ParentOrganisation.AddChild(accessTemplateViewModel);
					AllAccessTemplates.Add(accessTemplateViewModel);
					SelectedAccessTemplate = accessTemplateViewModel;
				}
			}
		}
		bool CanPaste()
		{
			return SelectedAccessTemplate != null && _clipboard != null && ParentOrganisation != null && ParentOrganisation.Organisation.UID == _clipboard.OrganisationUID;
		}

		AccessTemplate CopyAccessTemplate(AccessTemplate source, bool newName = true)
		{
			var copy = new AccessTemplate();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			foreach (var cardDoor in source.CardDoors)
			{
				var copyCardDoor = new CardDoor();
				copyCardDoor.DoorUID = cardDoor.DoorUID;
				copyCardDoor.EnterIntervalType = cardDoor.EnterIntervalType;
				copyCardDoor.EnterIntervalID = cardDoor.EnterIntervalID;
				copyCardDoor.ExitIntervalType = cardDoor.ExitIntervalType;
				copyCardDoor.ExitIntervalID = cardDoor.ExitIntervalID;
				copyCardDoor.CardUID = null;
				copyCardDoor.AccessTemplateUID = null;
				copy.CardDoors.Add(copyCardDoor);
			}
			return copy;
		}
	}
}