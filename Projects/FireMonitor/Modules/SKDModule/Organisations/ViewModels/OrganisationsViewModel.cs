using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class OrganisationsViewModel : BaseViewModel
	{
		public OrganisationsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			Organisations = new ObservableCollection<OrganisationViewModel>();
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					var organisationViewModel = new OrganisationViewModel(organisation);
					Organisations.Add(organisationViewModel);
				}
			}
			SelectedOrganisation = Organisations.FirstOrDefault();

			ValidateUsers();
		}

		ObservableCollection<OrganisationViewModel> _organisation;
		public ObservableCollection<OrganisationViewModel> Organisations
		{
			get { return _organisation; }
			set
			{
				_organisation = value;
				OnPropertyChanged("Organisations");
			}
		}

		OrganisationViewModel _selectedOrganisation;
		public OrganisationViewModel SelectedOrganisation
		{
			get { return _selectedOrganisation; }
			set
			{
				_selectedOrganisation = value;
				OnPropertyChanged("SelectedOrganisation");

				if (value != null)
					OrganisationZonesViewModel = new OrganisationZonesViewModel(SelectedOrganisation.Organisation);
				else
					OrganisationZonesViewModel = null;
			}
		}

		OrganisationZonesViewModel _organisationZonesViewModel;
		public OrganisationZonesViewModel OrganisationZonesViewModel
		{
			get { return _organisationZonesViewModel; }
			set
			{
				_organisationZonesViewModel = value;
				OnPropertyChanged("OrganisationZonesViewModel");
			}
		}

		bool CanEditRemove()
		{
			return SelectedOrganisation != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var organisationDetailsViewModel = new OrganisationDetailsViewModel(this);
			if (DialogService.ShowModalWindow(organisationDetailsViewModel))
			{
				var organisation = organisationDetailsViewModel.Organisation;
				var saveResult = OrganisationHelper.Save(organisation);
				if (saveResult == false)
					return;
				var organisationViewModel = new OrganisationViewModel(organisation);
				Organisations.Add(organisationViewModel);
				SelectedOrganisation = organisationViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить огранизацию " + SelectedOrganisation.Organisation.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				var organisation = SelectedOrganisation.Organisation;
				var removeResult = OrganisationHelper.MarkDeleted(organisation);
				if (removeResult == false)
					return;
				Organisations.Remove(SelectedOrganisation);
				SelectedOrganisation = Organisations.FirstOrDefault();
				ValidateUsers();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var organisationDetailsViewModel = new OrganisationDetailsViewModel(this, SelectedOrganisation.Organisation);
			if (DialogService.ShowModalWindow(organisationDetailsViewModel))
			{
				var organisation = organisationDetailsViewModel.Organisation;
				var saveResult = OrganisationHelper.Save(organisation);
				if (saveResult == false)
					return;
				SelectedOrganisation.Organisation = organisation;
				SelectedOrganisation.Update();
			}
		}

		void ValidateUsers()
		{
			foreach (var user in FiresecManager.SecurityConfiguration.Users)
			{
				var organisationUIDs = new List<Guid>();
				foreach (var organisationUID in user.OrganisationUIDs)
				{
					if (Organisations.Any(x => x.Organisation.UID == organisationUID))
						organisationUIDs.Add(organisationUID);
					//else
					//    ServiceFactory.SaveService.SecurityChanged = true;
				}
				user.OrganisationUIDs = organisationUIDs;
			}
		}
	}
}