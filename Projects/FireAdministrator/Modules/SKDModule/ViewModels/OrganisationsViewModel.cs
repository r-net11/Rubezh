using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Localization.SKD.ViewModels;
using StrazhAPI;
using StrazhAPI.SKD;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class OrganisationsViewModel : MenuViewPartViewModel
	{
		private LogicalDeletationType _logicalDeletationType;

		public bool IsWithDeleted { get { return _logicalDeletationType == LogicalDeletationType.All; } }

		public OrganisationsViewModel()
		{
			Menu = new OrganisationsMenuViewModel(this);

			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			RestoreCommand = new RelayCommand(OnRestore, CanRestore);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			ShowOrHideDeletedCommand = new RelayCommand(OnShowOrHideDeleted);
			ServiceFactoryBase.Events.GetEvent<SKDDoorsChangedEvent>().Subscribe(OnSKDDoorsChanged);
		}

		public void Initialize(LogicalDeletationType logicalDeletationType)
		{
			_logicalDeletationType = logicalDeletationType;
			OnPropertyChanged(() => IsWithDeleted);
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

		private ObservableCollection<OrganisationViewModel> _organisation;
		public ObservableCollection<OrganisationViewModel> Organisations
		{
			get { return _organisation; }
			set
			{
				_organisation = value;
				OnPropertyChanged(() => Organisations);
			}
		}

		private OrganisationViewModel _selectedOrganisation;
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
			}
		}

		private OrganisationDoorsViewModel _OrganisationDoorsViewModel;
		public OrganisationDoorsViewModel OrganisationDoorsViewModel
		{
			get { return _OrganisationDoorsViewModel; }
			set
			{
				_OrganisationDoorsViewModel = value;
				OnPropertyChanged(() => OrganisationDoorsViewModel);
			}
		}

		private OrganisationUsersViewModel _organisationUsersViewModel;
		public OrganisationUsersViewModel OrganisationUsersViewModel
		{
			get { return _organisationUsersViewModel; }
			set
			{
				_organisationUsersViewModel = value;
				OnPropertyChanged(() => OrganisationUsersViewModel);
			}
		}

		private bool CanEditRemove()
		{
			return SelectedOrganisation != null && !SelectedOrganisation.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
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
			}
		}
		private bool CanAdd()
		{
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			if (!MessageBoxService.ShowQuestion(CommonViewModels.DeleteOrganization_Question)) return;

			if (SelectedOrganisation == null) return;

			if (OrganisationHelper.IsAnyItems(SelectedOrganisation.Organisation.UID) &&
				!MessageBoxService.ShowQuestion(CommonViewModels.ArchiveBoundObjects_Question)) return;

			OrganisationHelper.MarkDeleted(SelectedOrganisation.Organisation);

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
		private void OnRestore()
		{
			if (MessageBoxService.ShowQuestion(CommonViewModels.RestoreOrganization_Question))
			{
				var restoreResult = OrganisationHelper.Restore(SelectedOrganisation.Organisation);
				if (!restoreResult)
					return;
				SelectedOrganisation.IsDeleted = false;
				SetItemsCanSelect(true);
			}
		}
		private bool CanRestore()
		{
			return SelectedOrganisation != null && SelectedOrganisation.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var organisationDetailsViewModel = new OrganisationDetailsViewModel(this, SelectedOrganisation.Organisation);
			if (DialogService.ShowModalWindow(organisationDetailsViewModel))
			{
				var organisation = organisationDetailsViewModel.Organisation;
				SelectedOrganisation.Organisation = organisation;
				SelectedOrganisation.Update();
			}
		}

		public RelayCommand ShowOrHideDeletedCommand { get; private set; }
		private void OnShowOrHideDeleted()
		{
			Initialize(IsWithDeleted ? LogicalDeletationType.Active : LogicalDeletationType.All);
			OnPropertyChanged(() => ShowOrHideDeletedCommandTooltip);
			OnPropertyChanged(() => ShowOrHideDeletedCommandImageSource);
		}

		public string ShowOrHideDeletedCommandTooltip
		{
			get { return IsWithDeleted ? CommonViewModels.ShowArchive : CommonViewModels.HideArchive; }
		}

		public string ShowOrHideDeletedCommandImageSource
		{
			get { return IsWithDeleted ? "ArchiveUnlocked" : "Archive"; }
		}

		private void SetItemsCanSelect(bool canSelect)
		{
			OrganisationDoorsViewModel.CanSelect = canSelect;
			OrganisationUsersViewModel.CanSelect = canSelect;
		}

		void OnSKDDoorsChanged(object unnecessaryObj)
		{
			SelectedOrganisation = SelectedOrganisation;
		}
	}
}