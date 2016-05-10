using System.Windows.Input;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Devices;

namespace StrazhModule.ViewModels
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
			DeviceAccessStateNormalCommand = new RelayCommand(OnDeviceAccessStateNormal, CanDeviceAccessStateNormal);
			DeviceAccessStateCloseAlwaysCommand = new RelayCommand(OnDeviceAccessStateCloseAlways, CanDeviceAccessStateCloseAlways);
			DeviceAccessStateOpenAlwaysCommand = new RelayCommand(OnDeviceAccessStateOpenAlways, CanDeviceAccessStateOpenAlways);
		}

		public bool CanControl
		{
			get { return Device.DriverType == SKDDriverType.Lock && FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control); }
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			DeviceCommander.Open(Device);
		}
		bool CanOpen()
		{
			return DeviceCommander.CanOpen(Device);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			DeviceCommander.Close(Device);
		}
		bool CanClose()
		{
			return DeviceCommander.CanClose(Device);
		}

		public RelayCommand DeviceAccessStateNormalCommand { get; private set; }
		void OnDeviceAccessStateNormal()
		{
			DeviceCommander.SetAccessStateToNormal(Device);
		}
		bool CanDeviceAccessStateNormal()
		{
			return DeviceCommander.CanSetAccessStateToNormal(Device);
		}

		public RelayCommand DeviceAccessStateCloseAlwaysCommand { get; private set; }
		void OnDeviceAccessStateCloseAlways()
		{
			DeviceCommander.SetAccessStateToCloseAlways(Device);
		}
		bool CanDeviceAccessStateCloseAlways()
		{
			return DeviceCommander.CanSetAccessStateToCloseAlways(Device);
		}

		public RelayCommand DeviceAccessStateOpenAlwaysCommand { get; private set; }
		void OnDeviceAccessStateOpenAlways()
		{
			DeviceCommander.SetAccessStateToOpenAlways(Device);
		}
		bool CanDeviceAccessStateOpenAlways()
		{
			return DeviceCommander.CanSetAccessStateToOpenAlways(Device);
		}

		void OnStateChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}