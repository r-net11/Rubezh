using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCDevicesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
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
		public List<OPCDeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<OPCDeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(OPCDeviceViewModel parentViewModel)
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

		OPCDeviceViewModel _selectedDevice;
		public OPCDeviceViewModel SelectedDevice
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

		OPCDeviceViewModel _rootDevice;
		public OPCDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged("RootDevice");
			}
		}

		public OPCDeviceViewModel[] RootDevices
		{
			get { return new OPCDeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		private OPCDeviceViewModel AddDeviceInternal(Device device, OPCDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new OPCDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		//public void Initialize()
		//{
		//    BuildTree();
		//    if (Devices.Count > 0)
		//    {
		//        CollapseChild(Devices[0]);
		//        ExpandChild(Devices[0]);
		//        SelectedDevice = Devices[0];
		//    }
		//}

		//#region DeviceSelection

		//public List<OPCDeviceViewModel> AllDevices;

		//public void FillAllDevices()
		//{
		//    AllDevices = new List<OPCDeviceViewModel>();
		//    AddChildPlainDevices(Devices[0]);
		//}

		//void AddChildPlainDevices(OPCDeviceViewModel parentViewModel)
		//{
		//    AllDevices.Add(parentViewModel);
		//    foreach (var childViewModel in parentViewModel.Children)
		//    {
		//        AddChildPlainDevices(childViewModel);
		//    }
		//}

		//public void Select(Guid deviceUID)
		//{
		//    Initialize();
		//    if (deviceUID != Guid.Empty)
		//    {
		//        FillAllDevices();

		//        var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
		//        if (deviceViewModel != null)
		//            deviceViewModel.ExpantToThis();
		//        SelectedDevice = deviceViewModel;
		//    }
		//}

		//#endregion

		//ObservableCollection<OPCDeviceViewModel> _devices;
		//public ObservableCollection<OPCDeviceViewModel> Devices
		//{
		//    get { return _devices; }
		//    set
		//    {
		//        _devices = value;
		//        OnPropertyChanged("Devices");
		//    }
		//}

		//OPCDeviceViewModel _selectedDevice;
		//public OPCDeviceViewModel SelectedDevice
		//{
		//    get { return _selectedDevice; }
		//    set
		//    {
		//        _selectedDevice = value;
		//        if (value != null)
		//            value.ExpantToThis();
		//        OnPropertyChanged("SelectedDevice");
		//    }
		//}

		//void BuildTree()
		//{
		//    Devices = new ObservableCollection<OPCDeviceViewModel>();
		//    AddDevice(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
		//}

		//public OPCDeviceViewModel AddDevice(Device device, OPCDeviceViewModel parentDeviceViewModel)
		//{
		//    var deviceViewModel = new OPCDeviceViewModel(device, Devices);
		//    deviceViewModel.Parent = parentDeviceViewModel;

		//    var indexOf = Devices.IndexOf(parentDeviceViewModel);
		//    Devices.Insert(indexOf + 1, deviceViewModel);

		//    foreach (var childDevice in device.Children)
		//    {
		//        var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
		//        deviceViewModel.Children.Add(childDeviceViewModel);
		//    }

		//    return deviceViewModel;
		//}

		//public void CollapseChild(OPCDeviceViewModel parentDeviceViewModel)
		//{
		//    parentDeviceViewModel.IsExpanded = false;
		//    foreach (var deviceViewModel in parentDeviceViewModel.Children)
		//    {
		//        CollapseChild(deviceViewModel);
		//    }
		//}

		//public void ExpandChild(OPCDeviceViewModel parentDeviceViewModel)
		//{
		//    if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
		//    {
		//        parentDeviceViewModel.IsExpanded = true;
		//        foreach (var deviceViewModel in parentDeviceViewModel.Children)
		//        {
		//            ExpandChild(deviceViewModel);
		//        }
		//    }
		//}

        public override void OnShow()
        {
            Initialize();
        }
	}
}