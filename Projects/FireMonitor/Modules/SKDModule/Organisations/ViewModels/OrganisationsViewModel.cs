using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class OrganisationsViewModel : BaseViewModel
	{
		LogicalDeletationType _logicalDeletationType;
		public bool IsWithDeleted { get { return _logicalDeletationType == LogicalDeletationType.All; } }

		public OrganisationsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			RestoreCommand = new RelayCommand(OnRestore, CanRestore);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
		}

		public void Initialize(LogicalDeletationType logicalDeletationType)
		{
			_logicalDeletationType = logicalDeletationType;
			OnPropertyChanged(() => IsWithDeleted);
			Organisations = new ObservableCollection<OrganisationViewModel>();
			var organisations = OrganisationHelper.Get(new OrganisationFilter {LogicalDeletationType = _logicalDeletationType });
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
					OrganisationDoorsViewModel = new OrganisationDoorsViewModel(SelectedOrganisation.Organisation);
					OrganisationUsersViewModel = new OrganisationUsersViewModel(SelectedOrganisation.Organisation);
				}
				else
				{
					OrganisationDoorsViewModel = null;
					OrganisationUsersViewModel = null;
				}

				HasOrganisationDoors = OrganisationDoorsViewModel != null && OrganisationDoorsViewModel.Items.Count > 0;
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
			return SelectedOrganisation != null && !SelectedOrganisation.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
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
				var currentUserViewModel = OrganisationUsersViewModel.Items.FirstOrDefault(x => x.User.UID == FiresecManager.CurrentUser.UID);
				if (currentUserViewModel != null && currentUserViewModel.User != null)
				{
					currentUserViewModel.SetWithoutSave(true);
				}
				ServiceFactoryBase.Events.GetEvent<NewOrganisationEvent>().Publish(SelectedOrganisation.Organisation.UID);
			}
		}
		bool CanAdd()
		{
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (!MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить организацию?")) return;

			if (SelectedOrganisation == null) return;

			if (OrganisationHelper.IsAnyItems(SelectedOrganisation.Organisation.UID) &&
			    !MessageBoxService.ShowQuestion("Привязанные к организации объекты будут также архивированы. Продолжить?")) return;

			OrganisationHelper.MarkDeleted(SelectedOrganisation.Organisation);
			ServiceFactoryBase.Events.GetEvent<RemoveOrganisationEvent>().Publish(SelectedOrganisation.Organisation.UID);

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
		}

		public RelayCommand RestoreCommand { get; private set; }
		void OnRestore()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите восстановить организацию?"))
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
			return SelectedOrganisation != null && SelectedOrganisation.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
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
			OrganisationDoorsViewModel.CanSelect = canSelect;
			OrganisationUsersViewModel.CanSelect = canSelect;
		}

		void OnUserChanged(UserChangedEventArgs args)
		{
			foreach (var organisation in Organisations.Select(x => x.Organisation))
			{
				ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Publish(organisation);
			}

		}
	}
}