using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class XDevicesViewModel : BaseViewModel
	{
		private GKModule.ViewModels.DevicesViewModel _devicesViewModel;

		public XDevicesViewModel(GKModule.ViewModels.DevicesViewModel devicesViewModel)
		{
			_devicesViewModel = devicesViewModel;
			_devicesViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "SelectedDevice")
					OnPropertyChanged(e.PropertyName);
			};

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}

		public void Select(Guid deviceUID)
		{
			_devicesViewModel.Select(deviceUID);
		}

		//private void BuildTree()
		//{
		//    AddDevice(XManager.DeviceConfiguration.RootDevice, null);
		//}
		//public XDeviceViewModel AddDevice(XDevice device, XDeviceViewModel parentDeviceViewModel)
		//{
		//    var deviceViewModel = new XDeviceViewModel(device, Devices);
		//    deviceViewModel.Parent = parentDeviceViewModel;

		//    var indexOf = Devices.IndexOf(parentDeviceViewModel);
		//    Devices.Insert(indexOf + 1, deviceViewModel);

		//    if (device != null)
		//        foreach (var childDevice in device.Children)
		//        {
		//            if (childDevice.Driver.IsDeviceOnShleif || childDevice.Children.Count > 0)
		//            {
		//                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
		//                deviceViewModel.Children.Add(childDeviceViewModel);
		//            }
		//        }

		//    return deviceViewModel;
		//}
		//private void AddChildPlainDevices(XDeviceViewModel parentViewModel)
		//{
		//    AllDevices.Add(parentViewModel);
		//    foreach (var childViewModel in parentViewModel.Children)
		//        AddChildPlainDevices(childViewModel);
		//}

		public ObservableCollection<GKModule.ViewModels.DeviceViewModel> Devices
		{
			get { return _devicesViewModel.Devices; }
		}
		public GKModule.ViewModels.DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
			set
			{
				_devicesViewModel.SelectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		private void OnDeviceChanged(Guid xdeviceUID)
		{
			var device = Devices.FirstOrDefault(x => x.Device.UID == xdeviceUID);
			if (device != null)
			{
				device.Update();
				device.ExpantToThis();
				SelectedDevice = device;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementXDevice>().ToList().ForEach(element => Helper.ResetXDevice(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			elements.ForEach(element =>
				{
					ElementXDevice elementDevice = element as ElementXDevice;
					if (elementDevice != null)
						OnDeviceChanged(elementDevice.XDeviceUID);
				});
		}
		private void OnElementSelected(ElementBase element)
		{
			ElementXDevice elementDevice = element as ElementXDevice;
			if (elementDevice != null)
				Select(elementDevice.XDeviceUID);
		}
	}
}