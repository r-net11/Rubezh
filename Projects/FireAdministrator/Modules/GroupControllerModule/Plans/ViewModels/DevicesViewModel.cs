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
using Infrustructure.Plans.Designer;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);
			//ServiceFactory.Events.GetEvent<ElementDeviceChangedEvent>().Unsubscribe(OnDeviceChanged);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			//ServiceFactory.Events.GetEvent<ElementDeviceChangedEvent>().Subscribe(OnDeviceChanged);
			AllDevices = new List<DeviceViewModel>();
			Devices = new ObservableCollection<DeviceViewModel>();

			Update();
		}

		public List<DeviceViewModel> AllDevices;

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
			{
				AddChildPlainDevices(childViewModel);
			}
		}

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

			BuildTree();
			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
		}

		private void BuildTree()
		{
			var xRootDevice = XManager.DeviceConfiguration.RootDevice;
			AddDevice(xRootDevice, null);
		}
		public DeviceViewModel AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
		{
			var xDeviceViewModel = new DeviceViewModel(xDevice, Devices);
			xDeviceViewModel.Parent = parentDeviceViewModel;

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, xDeviceViewModel);

			if (xDevice != null)
				foreach (var childDevice in xDevice.Children)
				{
					var childDeviceViewModel = AddDevice(childDevice, xDeviceViewModel);
					xDeviceViewModel.Children.Add(childDeviceViewModel);
				}

			return xDeviceViewModel;
		}


		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		private DeviceViewModel _selectedDevice;
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
				device.Update();
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementDevice>().ToList().ForEach(element =>
				{
					Device device = element.DeviceUID == null ? null : FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == element.DeviceUID);
					if (device != null)
						device.PlanElementUIDs.Remove(element.UID);
				});
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

		private void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				CollapseChild(deviceViewModel);
		}
		private void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				ExpandChild(deviceViewModel);
		}
	}
}
