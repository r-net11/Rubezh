using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
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
		SimpleDeviceViewModel AddDeviceInternal(GKDevice device, SimpleDeviceViewModel parentDeviceViewModel)
		{
			var smallDeviceViewModel = new SimpleDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(smallDeviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, smallDeviceViewModel);
			if (device.UID == _elementGKDevice.DeviceUID)
			{
				SelectedDevice = smallDeviceViewModel;
				CurrentDeviceUid = SelectedDevice.Device.UID;
			}
			return smallDeviceViewModel;
		}

		SimpleDeviceViewModel _rootDevice;
		public SimpleDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
				OnPropertyChanged(() => RootDevices);
			}
		}
		public SimpleDeviceViewModel[] RootDevices
		{
			get { return new SimpleDeviceViewModel[] { RootDevice }; }
		}

		SimpleDeviceViewModel _selectedDevice;
		public SimpleDeviceViewModel SelectedDevice
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
				return SelectedDevice != null ? SelectedDevice.Device.Description : "";
			}
			set
			{
				if (SelectedDevice != null)
				{
					SelectedDevice.Device.Description = value;
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
			if (SelectedDevice != null && SelectedDevice.Device.Driver.IsPlaceable)
			{
				if (SelectedDevice.Device.UID == CurrentDeviceUid)
					return true;
				if (SelectedDevice.Device.PlanElementUIDs.Count == 0 || SelectedDevice.Device.AllowMultipleVizualization)
					return true;
			}
			return false;
		}
	}
}