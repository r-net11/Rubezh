using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementGKDevice _elementGKDevice;
		Guid CurrentDeviceUid { get; set; }

		public DevicePropertiesViewModel(ElementGKDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementGKDevice = elementDevice;
			Left = (int)(_elementGKDevice.Left * _sensivityFactor);
			Top = (int)(_elementGKDevice.Top * _sensivityFactor);

			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}
		DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			if (device.UID == _elementGKDevice.DeviceUID)
			{
				SelectedDevice = deviceViewModel;
				CurrentDeviceUid = SelectedDevice.Device.UID;
			}
			return deviceViewModel;
		}

		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
				OnPropertyChanged(() => RootDevices);
			}
		}
		public DeviceViewModel[] RootDevices
		{
			get { return new DeviceViewModel[] { RootDevice }; }
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
		public string DeviceDescription
		{
			get
			{
				return SelectedDevice != null ? SelectedDevice.Description : "";
			}
			set
			{
				if (SelectedDevice != null)
				{
					SelectedDevice.Description = value;
					var device = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == SelectedDevice.Device);
					if (device != null)
						device.Update();
				}
				OnPropertyChanged(() => DeviceDescription);
			}
		}

		protected override bool Save()
		{
			_elementGKDevice.Left = (double)Left / _sensivityFactor;
			_elementGKDevice.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(_elementGKDevice, SelectedDevice.Device);
			return base.Save();
		}

		protected override bool CanSave()
		{
			if (SelectedDevice != null && SelectedDevice.Driver.IsPlaceable)
			{
				if (SelectedDevice.Device.UID == CurrentDeviceUid)
					return true;
				if (SelectedDevice.Device.PlanElementUIDs.Count == 0 || SelectedDevice.Device.AllowMultipleVizualization)
					return true;
			}
			return false;
		}

		public override void OnClosed()
		{
			Unsubscribe(RootDevices);
			base.OnClosed();
		}
		void Unsubscribe(IEnumerable<DeviceViewModel> devices)
		{
			foreach (var device in devices)
			{
				device.UnsubscribeEvents();
				Unsubscribe(device.Children);
			}
		}
	}
}