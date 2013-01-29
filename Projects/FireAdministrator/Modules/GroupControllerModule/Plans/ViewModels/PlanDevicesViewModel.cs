using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using GKModule.ViewModels;

namespace GKModule.Plans.ViewModels
{
	public class PlanDevicesViewModel : BaseViewModel
	{
		private bool _lockSelection;
		private DevicesViewModel _devicesViewModel;

		public PlanDevicesViewModel(DevicesViewModel devicesViewModel)
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
					device.ExpantToThis();
					SelectedDevice = device;
				}
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementXDevice>().ToList().ForEach(element => Helper.ResetXDevice(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				ElementXDevice elementDevice = element as ElementXDevice;
				if (elementDevice != null)
				{
					if (guid != Guid.Empty)
						OnDeviceChanged(guid);
					guid = elementDevice.XDeviceUID;
				}
			});
			_lockSelection = false;
			if (guid != Guid.Empty)
				OnDeviceChanged(guid);
		}

		private void OnElementSelected(ElementBase element)
		{
			ElementXDevice elementDevice = element as ElementXDevice;
			if (elementDevice != null)
				Select(elementDevice.XDeviceUID);
		}
	}
}