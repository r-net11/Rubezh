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

namespace DevicesModule.Plans.ViewModels
{
	public class DevicesViewModel : BaseViewModel
	{
		private Devices.DevicesViewModel _devicesViewModel;

		public DevicesViewModel(Devices.DevicesViewModel devicesViewModel)
		{
			_devicesViewModel = devicesViewModel;
			//_devicesViewModel.PropertyChanged += (s, e) =>
			//    {
			//        if (e.PropertyName == "SelectedDevice")
			//            OnPropertyChanged(e.PropertyName);
			//    };

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
		public Devices.DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
			set
			{
				_devicesViewModel.SelectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}
		public List<Devices.DeviceViewModel> AllDevices
		{
			get { return _devicesViewModel.AllDevices; }
		}

		private void OnDeviceChanged(Guid deviceUID)
		{
			var device = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
			{
				device.Update();
				device.ExpantToThis();
				SelectedDevice = device;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementDevice>().ToList().ForEach(element => Helper.ResetDevice(element));
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
	}
}
