using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Localization.Security.Common;
using Localization.Security.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SecurityModule.ViewModels
{
	public class RolesViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public RolesViewModel()
		{
			Menu = new RolesMenuViewModel(this);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			AddCommand = new RelayCommand(OnAdd);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Roles = new SortableObservableCollection<RoleViewModel>();
			foreach (var role in FiresecManager.SecurityConfiguration.UserRoles)
				Roles.Add(new RoleViewModel(role));
			SelectedRole = Roles.FirstOrDefault();
		}

		private SortableObservableCollection<RoleViewModel> _roles;
		public SortableObservableCollection<RoleViewModel> Roles
		{
			get { return _roles; }
			set
			{
				_roles = value;
				OnPropertyChanged(() => Roles);
			}
		}

		private RoleViewModel _selectedRole;
		public RoleViewModel SelectedRole
		{
			get { return _selectedRole; }
			set
			{
				_selectedRole = value;
				OnPropertyChanged(() => SelectedRole);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var roleDetailsViewModel = new RoleDetailsViewModel();
			if (DialogService.ShowModalWindow(roleDetailsViewModel))
			{
				FiresecManager.SecurityConfiguration.UserRoles.Add(roleDetailsViewModel.Role);
				var roleViewModel = new RoleViewModel(roleDetailsViewModel.Role);
				Roles.Add(roleViewModel);
				SelectedRole = roleViewModel;
				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(String.Format(CommonViewModels.Roles_DeleteCommand, SelectedRole.Role.Name)))
				return;
			FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
			Roles.Remove(SelectedRole);
			ServiceFactory.SaveService.SecurityChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var roleDetailsViewModel = new RoleDetailsViewModel(SelectedRole.Role);
			if (DialogService.ShowModalWindow(roleDetailsViewModel))
			{
				FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
				SelectedRole.Role = roleDetailsViewModel.Role;
				FiresecManager.SecurityConfiguration.UserRoles.Add(SelectedRole.Role);

				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		private bool CanEditDelete()
		{
			return SelectedRole != null;
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
				new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(CommonResources.Add, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(CommonResources.Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Delete, DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}

		public override void OnShow()
		{
			if(Roles != null)
				Roles.Sort(x => x.Role != null ? x.Role.Name : string.Empty);
			base.OnShow();
		}
	}
}