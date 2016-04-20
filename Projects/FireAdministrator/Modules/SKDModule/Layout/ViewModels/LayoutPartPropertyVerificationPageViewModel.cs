using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models.Layouts;
using RubezhAPI.SKD;
using Infrastructure.Common.Services.Layout;
using System;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class LayoutPartPropertyVerificationPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartVerificationViewModel _layoutPartVerificationViewModel;

		public LayoutPartPropertyVerificationPageViewModel(LayoutPartVerificationViewModel layoutPartFilterViewModel)
		{
			_layoutPartVerificationViewModel = layoutPartFilterViewModel;

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.IsCardReaderOrCodeReader)
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
		GKDevice GKDevice { get; set; }

		public Guid UID { get; private set; }
		public string Name { get; private set; }
		public string Address { get; private set; }

		public string NameAndAddress
		{
			get { return Name + " " + Address; }
		}

		public DeviceViewModel(GKDevice gkDevice)
		{
			GKDevice = gkDevice;
			UID = gkDevice.UID;
			Name = gkDevice.Driver.ShortName;
			if (!string.IsNullOrEmpty(gkDevice.Description))
				Name += " (" + gkDevice.Description + ")";
			Address = gkDevice.DottedAddress;
		}
	}
}