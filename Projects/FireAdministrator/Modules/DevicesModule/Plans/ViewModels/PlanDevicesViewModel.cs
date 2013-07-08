using System;
using System.Collections.Generic;
using System.Linq;
using DevicesModule.Plans.Designer;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Devices = DevicesModule.ViewModels;
using System.Windows;
using Infrastructure.Common;

namespace DevicesModule.Plans.ViewModels
{
	public class PlanDevicesViewModel : BaseViewModel
	{
		bool _lockSelection;
		DevicesViewModel _devicesViewModel;

		public PlanDevicesViewModel(DevicesViewModel devicesViewModel)
		{
			_lockSelection = false;
			_devicesViewModel = devicesViewModel;
			// TODO: FIX IT
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

		public DeviceViewModel[] RootDevices
		{
			get { return _devicesViewModel.RootDevices; }
		}
		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
			set
			{
				_devicesViewModel.SelectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}
		public List<DeviceViewModel> AllDevices
		{
			get { return _devicesViewModel.AllDevices; }
		}

		private void OnDeviceChanged(Guid deviceUID)
		{
			var device = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
			{
				device.Update();
				// TODO: FIX IT
				if (!_lockSelection)
				{
					device.ExpandToThis();
					SelectedDevice = device;
				}
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementDevice>().ToList().ForEach(element => Helper.ResetDevice(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				ElementDevice elementDevice = element as ElementDevice;
				if (elementDevice != null)
				{
					if (guid != Guid.Empty)
						OnDeviceChanged(guid);
					guid = elementDevice.DeviceUID;
				}
			});
			_lockSelection = false;
			if (guid != Guid.Empty)
				OnDeviceChanged(guid);
		}
		private void OnElementSelected(ElementBase element)
		{
			ElementDevice elementDevice = element as ElementDevice;
			if (elementDevice != null)
				Select(elementDevice.DeviceUID);
		}
	}
}