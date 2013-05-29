using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DevicesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
        public static DevicesViewModel Current { get; private set; }
        public DevicesViewModel()
        {
            Current = this;
        }

        public void Initialize()
        {
            BuildTree();
            if (RootDevice != null)
            {
                RootDevice.IsExpanded = true;
                SelectedDevice = RootDevice;
            }
            OnPropertyChanged("RootDevices");
        }

        #region DeviceSelection
        public List<DeviceViewModel> AllDevices;

        public void FillAllDevices()
        {
            AllDevices = new List<DeviceViewModel>();
            AddChildPlainDevices(RootDevice);
        }

        void AddChildPlainDevices(DeviceViewModel parentViewModel)
        {
            AllDevices.Add(parentViewModel);
            foreach (var childViewModel in parentViewModel.Children)
                AddChildPlainDevices(childViewModel);
        }

        public void Select(Guid deviceUID)
        {
            if (deviceUID != Guid.Empty)
            {
                var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
                if (deviceViewModel != null)
                    deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }
        #endregion

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        DeviceViewModel _rootDevice;
        public DeviceViewModel RootDevice
        {
            get { return _rootDevice; }
            private set
            {
                _rootDevice = value;
                OnPropertyChanged("RootDevice");
            }
        }

        public DeviceViewModel[] RootDevices
        {
            get { return new DeviceViewModel[] { RootDevice }; }
        }

        void BuildTree()
        {
            RootDevice = AddDeviceInternal(XManager.DeviceConfiguration.RootDevice, null);
            FillAllDevices();
        }

        private DeviceViewModel AddDeviceInternal(XDevice device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            if (parentDeviceViewModel != null)
                parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (!childDevice.IsNotUsed)
				{
					AddDeviceInternal(childDevice, deviceViewModel);
				}
			}
            return deviceViewModel;
        }
	}
}