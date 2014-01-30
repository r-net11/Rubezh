using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using Infrastructure;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceState DeviceState
		{
			get { return Device.State; }
		}

		public DeviceCommandsViewModel(SKDDevice device)
		{
			Device = device;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);

			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommandBits)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
		}

		public bool CanControl
		{
			get { return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.SKDOpenDevice(Device);
            }
		}
		bool CanOpen()
		{
			return true;
		}

		public RelayCommand CloseCommand { get; private set; }
        void OnClose()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.SKDCloseDevice(Device);
            }
        }
		bool CanClose()
		{
			return true;
		}

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}
	}
}