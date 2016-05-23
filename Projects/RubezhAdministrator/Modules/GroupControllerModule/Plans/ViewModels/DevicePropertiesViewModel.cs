using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDevice _elementGKDevice;
		Guid CurrentDeviceUid { get; set; }
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public DevicePropertiesViewModel(ElementGKDevice elementDevice, CommonDesignerCanvas designerCanvas)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementGKDevice = elementDevice;
			PositionSettingsViewModel = new PositionSettingsViewModel(_elementGKDevice as ElementBase, designerCanvas);

			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}

		SimpleDeviceViewModel AddDeviceInternal(GKDevice device, SimpleDeviceViewModel parentDeviceViewModel)
		{
			var simpleDeviceViewModel = new SimpleDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(simpleDeviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, simpleDeviceViewModel);
			if (device.UID == _elementGKDevice.DeviceUID)
			{
				SelectedDevice = simpleDeviceViewModel;
				CurrentDeviceUid = SelectedDevice.Device.UID;
			}
			return simpleDeviceViewModel;
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
			PositionSettingsViewModel.SavePosition();
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