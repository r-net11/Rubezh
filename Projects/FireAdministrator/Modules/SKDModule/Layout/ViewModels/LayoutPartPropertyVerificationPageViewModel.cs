using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.Models.Layouts;
using StrazhAPI.SKD;
using Infrastructure.Common.Services.Layout;
using System;
using StrazhAPI.GK;
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
		}

		public override string Header
		{
			get { return CommonViewModels.VerificationSettings; }
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
				_layoutPartVerificationViewModel.UpdateLayoutPart(CommonViewModels.UnknownDevice);
			}
			return true;
		}
	}

	public class DeviceViewModel
	{
		SKDDevice SKDDevice { get; set; }

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
	}
}