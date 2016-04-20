using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SecurityModule.ViewModels
{
	public class RolesViewModel : MenuViewPartViewModel
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
			Roles = new ObservableCollection<RoleViewModel>();
			foreach (var role in ClientManager.SecurityConfiguration.UserRoles)
				Roles.Add(new RoleViewModel(role));
			SelectedRole = Roles.FirstOrDefault();
		}

		ObservableCollection<RoleViewModel> _roles;
		public ObservableCollection<RoleViewModel> Roles
		{
			get { return _roles; }
			set
			{
				_roles = value;
				OnPropertyChanged(() => Roles);
			}
		}

		RoleViewModel _selectedRole;
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
		void OnAdd()
		{
			var roleDetailsViewModel = new RoleDetailsViewModel();
			if (DialogService.ShowModalWindow(roleDetailsViewModel))
			{
				ClientManager.SecurityConfiguration.UserRoles.Add(roleDetailsViewModel.Role);
				var roleViewModel = new RoleViewModel(roleDetailsViewModel.Role);
				Roles.Add(roleViewModel);
				SelectedRole = roleViewModel;
				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить шаблон прав \"{0}\" из списка?", SelectedRole.Role.Name)))
			{
				ClientManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
				Roles.Remove(SelectedRole);
				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var roleDetailsViewModel = new RoleDetailsViewModel(SelectedRole.Role);
			if (DialogService.ShowModalWindow(roleDetailsViewModel))
			{
				ClientManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
				SelectedRole.Role = roleDetailsViewModel.Role;
				ClientManager.SecurityConfiguration.UserRoles.Add(SelectedRole.Role);

				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		bool CanEditDelete()
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
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить (Ctrl+N)", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать (Ctrl+E)", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить (Ctrl+Del)", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}