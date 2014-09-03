using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

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
			Organisations = new ObservableCollection<OrganisationViewModel>();
			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			if (organisations != null)
			{
				foreach (var organisation in organisations)
				{
					var organisationViewModel = new OrganisationViewModel(organisation);
					Organisations.Add(organisationViewModel);
				}
			}
			SelectedOrganisation = Organisations.FirstOrDefault();
		}

		ObservableCollection<OrganisationViewModel> _organisation;
		public ObservableCollection<OrganisationViewModel> Organisations
		{
			get { return _organisation; }
			set
			{
				_organisation = value;
				OnPropertyChanged(() => Organisations);
			}
		}

		OrganisationViewModel _selectedOrganisation;
		public OrganisationViewModel SelectedOrganisation
		{
			get { return _selectedOrganisation; }
			set
			{
				_selectedOrganisation = value;
				OnPropertyChanged(() => SelectedOrganisation);

				if (value != null)
				{
					OrganisationZonesViewModel = new OrganisationZonesViewModel(SelectedOrganisation.Organisation);
					OrganisationDoorsViewModel = new OrganisationDoorsViewModel(SelectedOrganisation.Organisation);
					OrganisationGuardZonesViewModel = new OrganisationGuardZonesViewModel(SelectedOrganisation.Organisation);
					OrganisationUsersViewModel = new OrganisationUsersViewModel(SelectedOrganisation.Organisation);
					OrganisationCardTemplatesViewModel = new OrganisationCardTemplatesViewModel(SelectedOrganisation.Organisation);
				}
				else
				{
					OrganisationZonesViewModel = null;
					OrganisationDoorsViewModel = null;
					OrganisationGuardZonesViewModel = null;
					OrganisationUsersViewModel = null;
					OrganisationCardTemplatesViewModel = null;
				}
			}
		}

		OrganisationZonesViewModel _organisationZonesViewModel;
		public OrganisationZonesViewModel OrganisationZonesViewModel
		{
			get { return _organisationZonesViewModel; }
			set
			{
				_organisationZonesViewModel = value;
				OnPropertyChanged(() => OrganisationZonesViewModel);
			}
		}

		OrganisationDoorsViewModel _OrganisationDoorsViewModel;
		public OrganisationDoorsViewModel OrganisationDoorsViewModel
		{
			get { return _OrganisationDoorsViewModel; }
			set
			{
				_OrganisationDoorsViewModel = value;
				OnPropertyChanged(() => OrganisationDoorsViewModel);
			}
		}

		OrganisationGuardZonesViewModel _organisationGuardZonesViewModel;
		public OrganisationGuardZonesViewModel OrganisationGuardZonesViewModel
		{
			get { return _organisationGuardZonesViewModel; }
			set
			{
				_organisationGuardZonesViewModel = value;
				OnPropertyChanged(() => OrganisationGuardZonesViewModel);
			}
		}

		OrganisationUsersViewModel _organisationUsersViewModel;
		public OrganisationUsersViewModel OrganisationUsersViewModel
		{
			get { return _organisationUsersViewModel; }
			set
			{
				_organisationUsersViewModel = value;
				OnPropertyChanged(() => OrganisationUsersViewModel);
			}
		}

		OrganisationCardTemplatesViewModel _organisationCardTemplatesViewModel;
		public OrganisationCardTemplatesViewModel OrganisationCardTemplatesViewModel
		{
			get { return _organisationCardTemplatesViewModel; }
			set
			{
				_organisationCardTemplatesViewModel = value;
				OnPropertyChanged(() => OrganisationCardTemplatesViewModel);
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
				var organisationViewModel = new OrganisationViewModel(organisation);
				Organisations.Add(organisationViewModel);
				SelectedOrganisation = organisationViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion2("Вы уверены, что хотите удалить огранизацию?"))
			{
				var organisation = SelectedOrganisation.Organisation;
				var removeResult = OrganisationHelper.MarkDeleted(organisation);
				if (removeResult == false)
					return;
				Organisations.Remove(SelectedOrganisation);
				SelectedOrganisation = Organisations.FirstOrDefault();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var organisationDetailsViewModel = new OrganisationDetailsViewModel(this, SelectedOrganisation.Organisation);
			if (DialogService.ShowModalWindow(organisationDetailsViewModel))
			{
				var organisation = organisationDetailsViewModel.Organisation;
				SelectedOrganisation.Organisation = organisation;
				SelectedOrganisation.Update();
				ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Publish(organisation);
            }
		}
	}
}