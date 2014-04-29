using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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

			SetRegimeOpenCommand = new RelayCommand(OnSetRegimeOpen, CanSetRegimeOpen);
			SetRegimeCloseCommand = new RelayCommand(OnSetRegimeClose, CanSetRegimeClose);
			SetRegimeControlCommand = new RelayCommand(OnSetRegimeControl, CanSetRegimeControl);
			SetRegimeConversationCommand = new RelayCommand(OnSetRegimeConversation, CanSetRegimeConversation);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
		}

		public bool CanControl
		{
			get { return Device.DriverType == SKDDriverType.Controller && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		public RelayCommand SetRegimeOpenCommand { get; private set; }
		void OnSetRegimeOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDSetRegimeOpen(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanSetRegimeOpen()
		{
			return true;
		}

		public RelayCommand SetRegimeCloseCommand { get; private set; }
		void OnSetRegimeClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDSetRegimeClose(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanSetRegimeClose()
		{
			return true;
		}

		public RelayCommand SetRegimeControlCommand { get; private set; }
		void OnSetRegimeControl()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDSetRegimeControl(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanSetRegimeControl()
		{
			return true;
		}

		public RelayCommand SetRegimeConversationCommand { get; private set; }
		void OnSetRegimeConversation()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDSetRegimeConversation(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanSetRegimeConversation()
		{
			return true;
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDevice(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
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
				var result = FiresecManager.FiresecService.SKDCloseDevice(Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanClose()
		{
			return true;
		}

		void OnStateChanged()
		{
		}
	}
}