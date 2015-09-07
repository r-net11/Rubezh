using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using FiresecAPI.SKD.Device;
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

		public bool ValidateIfPasswordAlreadyExists(string password)
		{
			return Passwords.Any(x => x.Password == password);
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
	}
}
