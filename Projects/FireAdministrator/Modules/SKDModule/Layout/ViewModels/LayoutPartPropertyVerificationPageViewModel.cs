﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using Infrastructure.Common.Services.Layout;
using System;
using FiresecAPI.GK;
using FiresecClient;

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
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartVerificationViewModel.Properties;

			if (SelectedDevice != null)
			{
				properties.ReferenceUID = SelectedDevice.UID;
				_layoutPartVerificationViewModel.UpdateLayoutPart(SelectedDevice.NameAndAddress);
			}
			else
			{
				properties.ReferenceUID = Guid.Empty;
				_layoutPartVerificationViewModel.UpdateLayoutPart("Устройство не указано");
			}
			return true;
		}
	}

	public class DeviceViewModel
	{
		SKDDevice SKDDevice { get; set; }
		GKDevice GKDevice { get; set; }

		public Guid UID { get; private set; }
		public string Name { get; private set; }
		public string Address { get; private set; }

		public string NameAndAddress
		{
			get { return Name + " " + Address; }
		}

		public DeviceViewModel(SKDDevice skdDevice)
		{
			SKDDevice = skdDevice;
			UID = skdDevice.UID;
			Address = skdDevice.Parent != null ? skdDevice.Parent.Address : "";
			Name = skdDevice.Name;
		}

		public DeviceViewModel(GKDevice gkDevice)
		{
			GKDevice = gkDevice;
			UID = gkDevice.UID;
			Name = gkDevice.Driver.ShortName;
			Address = gkDevice.PresentationAddress;
		}
	}
}