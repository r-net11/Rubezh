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
using GKModule.Plans.Designer;

namespace GKModule.Plans.ViewModels
{
	public class XDevicesViewModel : BaseViewModel
	{
		public XDevicesViewModel()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			AllDevices = new List<XDeviceViewModel>();
			Devices = new ObservableCollection<XDeviceViewModel>();

			Update();
		}

		public List<XDeviceViewModel> AllDevices;

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
				AddChildPlainDevices(Devices[0]);
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
		public XDeviceViewModel AddDevice(XDevice device, XDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new XDeviceViewModel(device, Devices);
			deviceViewModel.Parent = parentDeviceViewModel;

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, deviceViewModel);

			if (device != null)
				foreach (var childDevice in device.Children)
				{
					if (!childDevice.Driver.IsDeviceOnShleif && childDevice.Children.Count == 0)
						continue;

					var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
					deviceViewModel.Children.Add(childDeviceViewModel);
				}

			return deviceViewModel;
		}
		private void AddChildPlainDevices(XDeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public ObservableCollection<XDeviceViewModel> Devices { get; private set; }

		private XDeviceViewModel _selectedDevice;
		public XDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		private void Update(ElementXDevice elementDevice)
		{
			var device = Devices.FirstOrDefault(x => x.Device.UID == elementDevice.XDeviceUID);
			if (device != null)
				device.Update();
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
					{
						Update(elementDevice);
						Select(elementDevice.UID);
					}
				});
		}
		private void OnElementSelected(ElementBase element)
		{
			ElementXDevice elementDevice = element as ElementXDevice;
			if (elementDevice != null)
				Select(elementDevice.XDeviceUID);
		}

		private void CollapseChild(XDeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				CollapseChild(deviceViewModel);
		}
		private void ExpandChild(XDeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				ExpandChild(deviceViewModel);
		}
	}
}