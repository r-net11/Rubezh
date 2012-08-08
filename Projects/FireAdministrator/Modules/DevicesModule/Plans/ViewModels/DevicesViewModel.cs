using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace DevicesModule.Plans.ViewModels
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			AllDevices = new List<DeviceViewModel>();
			Devices = new ObservableCollection<DeviceViewModel>();

			Update();
		}

		public List<DeviceViewModel> AllDevices;

		//void AddChildPlainDevices(DeviceViewModel parentViewModel)
		//{
		//    AllDevices.Add(parentViewModel);
		//    foreach (var childViewModel in parentViewModel.Children)
		//    {
		//        AddChildPlainDevices(childViewModel);
		//    }
		//}

		public void Select(Guid deviceUID)
		{
			var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceViewModel != null)
				deviceViewModel.ExpantToThis();
			SelectedDevice = deviceViewModel;
		}

		public void Update()
		{
			AllDevices.Clear();
			Devices.Clear();

			foreach (var device in FiresecManager.Devices)
			{
				if (device.IsNotUsed)
					continue;

				if (!device.Driver.IsPlaceable && device.Children.Count == 0)
					continue;

				var deviceViewModel = new DeviceViewModel(device, Devices);
				deviceViewModel.IsExpanded = true;
				Devices.Add(deviceViewModel);
				AllDevices.Add(deviceViewModel);
			}

			foreach (var device in Devices)
			{
				if (device.Device.Parent != null)
				{
					var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					device.Parent = parent;
					parent.Children.Add(device);
				}
			}

			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		private void OnDeviceChanged(Guid deviceUID)
		{
			var device = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
			{
				device.Update();
				device.ExpantToThis();
				SelectedDevice = device;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			elements.ForEach(element =>
				{
					ElementDevice elementDevice = element as ElementDevice;
					if (elementDevice != null)
						OnDeviceChanged(elementDevice.DeviceUID);
				});
		}

		private void OnElementSelected(ElementBase element)
		{
			ElementDevice elementDevice = element as ElementDevice;
			if (elementDevice != null)
				Select(elementDevice.DeviceUID);
		}

		void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;

			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				CollapseChild(deviceViewModel);
			}
		}

		void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}
	}
}
