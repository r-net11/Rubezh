using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ControllerLocksPasswordsViewModel : SaveCancelDialogViewModel
	{
		private readonly DeviceViewModel _deviceViewModel;

		public bool HasChanged { get; set; }

		public ObservableCollection<ControllerLocksPasswordViewModel> Passwords { get; private set; }

		private ControllerLocksPasswordViewModel _selectedPassword;
		public ControllerLocksPasswordViewModel SelectedPassword
		{
			get { return _selectedPassword; }
			set
			{
				if (_selectedPassword == value)
					return;
				_selectedPassword = value;
				OnPropertyChanged(() => SelectedPassword);
			}
		}

		public ControllerLocksPasswordsViewModel(DeviceViewModel deviceViewModel)
		{
			_deviceViewModel = deviceViewModel;

			Title = "Пароли замков";

			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			ReadCommand = new RelayCommand(OnRead);
			WriteCommand = new RelayCommand(OnWrite);

			Passwords = new ObservableCollection<ControllerLocksPasswordViewModel>();
			if (_deviceViewModel.Device.ControllerPasswords != null)
			{
				foreach (var password in _deviceViewModel.Device.ControllerPasswords.LocksPasswords)
				{
					var controllerLocksPasswordViewModel = new ControllerLocksPasswordViewModel(this)
					{
						Password = password.Password,
						IsAppliedToLock1 = password.IsAppliedToLock1,
						IsAppliedToLock2 = password.IsAppliedToLock2,
						IsAppliedToLock3 = password.IsAppliedToLock3,
						IsAppliedToLock4 = password.IsAppliedToLock4
					};
					Passwords.Add(controllerLocksPasswordViewModel);
				}
			}

			SelectedPassword = Passwords.FirstOrDefault();
		}

		public bool ValidateIfPasswordAlreadyExists(ControllerLocksPasswordViewModel locksPasswordViewModel, string password)
		{
			return Passwords
				.Where(x => x != locksPasswordViewModel)
				.Any(y => y.Password == password);
		}

		public SKDDevice Controller
		{
			get { return _deviceViewModel.Device; }
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var controllerLocksPasswordViewModel = new ControllerLocksPasswordViewModel(this);
			var controllerPasswordViewModel = new ControllerPasswordViewModel(controllerLocksPasswordViewModel);
			if (DialogService.ShowModalWindow(controllerPasswordViewModel))
			{
				Passwords.Add(controllerLocksPasswordViewModel);
				SelectedPassword = controllerLocksPasswordViewModel;
				HasChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var controllerPasswordViewModel = new ControllerPasswordViewModel(SelectedPassword);
			if (DialogService.ShowModalWindow(controllerPasswordViewModel))
			{
				HasChanged = true;
			}
		}
		private bool CanEdit()
		{
			return SelectedPassword != null;
		}


		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			Passwords.Remove(SelectedPassword);
			HasChanged = true;
		}
		private bool CanDelete()
		{
			return SelectedPassword != null;
		}

		public RelayCommand ReadCommand { get; private set; }
		private void OnRead()
		{
			var result = FiresecManager.FiresecService.GetControllerLocksPasswords(_deviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				// Очищаем список паролей замков
				Passwords.Clear();

				// Заполняем список паролями замков, считанными с контроллера
				var locksPasswords = result.Result;

				foreach (var locksPassword in locksPasswords)
				{
					var locksPasswordViewModel = new ControllerLocksPasswordViewModel(this)
					{
						Password = locksPassword.Password,
						IsAppliedToLock1 = locksPassword.IsAppliedToLock1,
						IsAppliedToLock2 = locksPassword.IsAppliedToLock2,
						IsAppliedToLock3 = locksPassword.IsAppliedToLock3,
						IsAppliedToLock4 = locksPassword.IsAppliedToLock4,
					};
					Passwords.Add(locksPasswordViewModel);
				}
				HasChanged = true;
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		private void OnWrite()
		{
			var locksPasswords = new List<SKDLocksPassword>();
			foreach (var locksPasswordViewModel in Passwords)
			{
				var locksPassword = new SKDLocksPassword()
				{
					Password = locksPasswordViewModel.Password,
					IsAppliedToLock1 = locksPasswordViewModel.IsAppliedToLock1,
					IsAppliedToLock2 = locksPasswordViewModel.IsAppliedToLock2,
					IsAppliedToLock3 = locksPasswordViewModel.IsAppliedToLock3,
					IsAppliedToLock4 = locksPasswordViewModel.IsAppliedToLock4,
				};
				locksPasswords.Add(locksPassword);
			}

			var result = FiresecManager.FiresecService.SetControllerLocksPasswords(_deviceViewModel.Device, locksPasswords);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			//HasChanged = true;
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				var controllerPasswords = new SKDControllerPasswords();
				foreach (var password in Passwords)
				{
					var locksPassword = new SKDLocksPassword()
					{
						Password = password.Password,
						IsAppliedToLock1 = password.IsAppliedToLock1,
						IsAppliedToLock2 = password.IsAppliedToLock2,
						IsAppliedToLock3 = password.IsAppliedToLock3,
						IsAppliedToLock4 = password.IsAppliedToLock4
					};
					controllerPasswords.LocksPasswords.Add(locksPassword);
				}
				_deviceViewModel.Device.ControllerPasswords = controllerPasswords;
				ServiceFactory.SaveService.SKDChanged = true;
			}
			return base.Save();
		}

		public bool IsChinaController4
		{
			get { return _deviceViewModel.Device.DriverType == SKDDriverType.ChinaController_4; }
		}
	}
}
