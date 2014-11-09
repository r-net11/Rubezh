using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
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
		LogicalDeletationType _logicalDeletationType;
		
		public OrganisationsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			RestoreCommand = new RelayCommand(OnRestore, CanRestore);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
		}

		public void Initialize(LogicalDeletationType logicalDeletationType)
		{
			_logicalDeletationType = logicalDeletationType;
			Organisations = new ObservableCollection<OrganisationViewModel>();
			var organisations = OrganisationHelper.Get(new OrganisationFilter { LogicalDeletationType = _logicalDeletationType });
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
					OrganisationUsersViewModel = new OrganisationUsersViewModel(SelectedOrganisation.Organisation);
				}
				else
				{
					OrganisationZonesViewModel = null;
					OrganisationDoorsViewModel = null;
					OrganisationUsersViewModel = null;
				}

				HasOrganisationZones = OrganisationZonesViewModel != null && OrganisationZonesViewModel.Items.Count > 0;
				HasOrganisationDoors = OrganisationDoorsViewModel != null && OrganisationDoorsViewModel.Items.Count > 0;
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

		bool _hasOrganisationZones;
		public bool HasOrganisationZones
		{
			get { return _hasOrganisationZones; }
			set
			{
				_hasOrganisationZones = value;
				OnPropertyChanged(() => HasOrganisationZones);
			}
		}

		bool _hasOrganisationDoors;
		public bool HasOrganisationDoors
		{
			get { return _hasOrganisationDoors; }
			set
			{
				_hasOrganisationDoors = value;
				OnPropertyChanged(() => HasOrganisationDoors);
			}
		}

		bool CanEditRemove()
		{
			return SelectedOrganisation != null && !SelectedOrganisation.IsDeleted;
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
				//var currentUserViewModel = OrganisationUsersViewModel.Items.FirstOrDefault(x => x.User.UID == FiresecManager.CurrentUser.UID);
				//if (currentUserViewModel.User != null)
				//{
				//    currentUserViewModel.IsChecked = true;
				//}
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить огранизацию?"))
			{
				if (!OrganisationHelper.IsAnyItems(SelectedOrganisation.Organisation.UID) ||
					MessageBoxService.ShowQuestion("Привязанные к организации объекты будут также архивированы. Продолжить?"))
				{
					var organisation = SelectedOrganisation.Organisation;
					var removeResult = OrganisationHelper.MarkDeleted(organisation);
					if (removeResult == false)
						return;
					if (_logicalDeletationType == LogicalDeletationType.All)
					{
						SelectedOrganisation.IsDeleted = true;
						SetItemsCanSelect(false);
					}
					else
					{
						Organisations.Remove(SelectedOrganisation);
						SelectedOrganisation = Organisations.FirstOrDefault();
					}
					ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Publish(organisation.UID);
				}
			}
		}
		
		public RelayCommand RestoreCommand { get; private set; }
		void OnRestore()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите восстановить огранизацию?"))
			{
				var restoreResult = OrganisationHelper.Restore(SelectedOrganisation.Organisation);
				if (!restoreResult)
					return;
				SelectedOrganisation.IsDeleted = false;
				SetItemsCanSelect(true);
				ServiceFactory.Events.GetEvent<RestoreOrganisationEvent>().Publish(SelectedOrganisation.Organisation.UID);
			}
		}
		bool CanRestore()
		{
			return SelectedOrganisation != null && SelectedOrganisation.IsDeleted;
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

		void SetItemsCanSelect(bool canSelect)
		{
			OrganisationZonesViewModel.CanSelect = canSelect;
			OrganisationDoorsViewModel.CanSelect = canSelect;
			OrganisationUsersViewModel.CanSelect = canSelect;
		}
	}
}