﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Services.Layout;

namespace SKDModule.ViewModels
{
	public class LayoutPartPropertyVerificationPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartVerificationViewModel _layoutPartVerificationViewModel;

		public LayoutPartPropertyVerificationPageViewModel(LayoutPartVerificationViewModel layoutPartFilterViewModel)
		{
			_layoutPartVerificationViewModel = layoutPartFilterViewModel;

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Reader)
				{
					var deviceViewModel = new DeviceViewModel(device);
					Devices.Add(deviceViewModel);
				}
			}
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
				{
					var deviceViewModel = new DeviceViewModel(device);
					Devices.Add(deviceViewModel);
				}
			}
		}

		public override string Header
		{
			get { return "Настройка верификации"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartVerificationViewModel.Properties;
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == properties.ReferenceUID);
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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

		public override bool CanSave()
		{
			return SelectedDevice != null;
		}
		public override bool Save()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartVerificationViewModel.Properties;

			properties.ReferenceUID = SelectedDevice == null ? Guid.Empty : SelectedDevice.UID;
			_layoutPartVerificationViewModel.UpdateLayoutPart(SelectedDevice.Name);
			return true;
		}
	}

	public class DeviceViewModel
	{
		public SKDDevice SKDDevice { get; private set; }
		public GKDevice GKDevice { get; private set; }

		public Guid UID { get; private set; }
		public string Name { get; private set; }

		public DeviceViewModel(SKDDevice skdDevice)
		{
			SKDDevice = skdDevice;
			UID = skdDevice.UID;
			Name = skdDevice.Name;
		}

		public DeviceViewModel(GKDevice gkDevice)
		{
			GKDevice = gkDevice;
			UID = gkDevice.UID;
			Name = gkDevice.PresentationName;
		}
	}
}