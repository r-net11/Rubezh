using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationsViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public OrganisationsViewModel()
		{
			Menu = new OrganisationsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RefreshCommand = new RelayCommand(OnRefresh);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
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
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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

		bool CanEditDelete()
		{
			return SelectedOrganisation != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var organisationDetailsViewModel = new OrganisationDetailsViewModel(this);
			if (DialogService.ShowModalWindow(organisationDetailsViewModel))
			{
				var organization = organisationDetailsViewModel.Organisation;
				var saveResult = OrganizationHelper.Save(organization);
				if (saveResult == false)
					return;
				var organisationViewModel = new OrganisationViewModel(organization);
				Organisations.Add(organisationViewModel);
				SelectedOrganisation = organisationViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить огранизацию " + SelectedOrganisation.Organisation.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				var organization = SelectedOrganisation.Organisation;
				var removeResult = OrganizationHelper.MarkDeleted(organization);
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
				var organization = organisationDetailsViewModel.Organisation;
				var saveResult = OrganizationHelper.Save(organization);
				if (saveResult == false)
					return;
				SelectedOrganisation.Organisation = organization;
				SelectedOrganisation.Update();
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedOrganisation = SelectedOrganisation;
		}

		public override void OnHide()
		{
			base.OnHide();
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}


		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}