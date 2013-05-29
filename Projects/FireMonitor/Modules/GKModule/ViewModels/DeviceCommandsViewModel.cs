using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
			ResetCommand = new RelayCommand(OnReset, CanReset);

			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommands)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
		}

        public bool IsTriStateControl
        {
			get { return Device.Driver.IsControlDevice; }
        }

        public bool IsBiStateControl
        {
            get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice; }
        }

        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (DeviceState.States.Contains(XStateType.Ignore))
                    return DeviceControlRegime.Ignore;

                if (DeviceState.States.Contains(XStateType.Norm))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }

		public bool IsControlRegime
		{
            get { return ControlRegime == DeviceControlRegime.Manual; }
		}

        public RelayCommand SetAutomaticStateCommand { get; private set; }
        void OnSetAutomaticState()
        {
			ObjectCommandSendHelper.SetAutomaticRegimeForDevice(Device);
        }

        public RelayCommand SetManualStateCommand { get; private set; }
        void OnSetManualState()
        {
			ObjectCommandSendHelper.SetManualRegimeForDevice(Device);
        }

        public RelayCommand SetIgnoreStateCommand { get; private set; }
        void OnSetIgnoreState()
        {
			ObjectCommandSendHelper.SetIgnoreRegimeForDevice(Device);
        }

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}

		public bool HasReset
		{
            get { return Device.Driver.DriverType == XDriverType.AMP_1 || Device.Driver.DriverType == XDriverType.RSR2_MAP4; }
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			ObjectCommandSendHelper.ResetDevice(Device);
		}
		bool CanReset()
		{
			return DeviceState.States.Contains(XStateType.Fire2) || DeviceState.States.Contains(XStateType.Fire1);
		}
	}
}