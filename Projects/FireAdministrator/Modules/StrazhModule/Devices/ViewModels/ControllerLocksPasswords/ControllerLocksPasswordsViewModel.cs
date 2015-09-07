using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ControllerLocksPasswordsViewModel : SaveCancelDialogViewModel
	{
		private DeviceViewModel _deviceViewModel;

		public ControllerLocksPasswordsViewModel(DeviceViewModel deviceViewModel)
		{
			_deviceViewModel = deviceViewModel;

			Title = "Пароли замков";

			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit);
			DeleteCommand = new RelayCommand(OnDelete);
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var controllerLockPasswordViewModel = new ControllerLockPasswordViewModel(this);
			if (DialogService.ShowModalWindow(controllerLockPasswordViewModel))
			{
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var controllerLockPasswordViewModel = new ControllerLockPasswordViewModel(this);
			if (DialogService.ShowModalWindow(controllerLockPasswordViewModel))
			{
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
		}

		public bool IsLock3And4Enabled {
			get { return _deviceViewModel.Driver.DriverType == SKDDriverType.ChinaController_4; }
		}
	}
}
