using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorDetailsViewModel : SaveCancelDialogViewModel
	{
		Device Device;
		List<int> Zones;

		public IndicatorDetailsViewModel(Device device)
		{
			Title = "Свойства индикатора";
			ShowZonesCommand = new RelayCommand(OnShowZones);
			ShowDevicesCommand = new RelayCommand(OnShowDevices);
			ResetDeviceCommand = new RelayCommand(OnResetDevice);

			OnColor = IndicatorColorType.Red;
			OffColor = IndicatorColorType.Green;
			FailureColor = IndicatorColorType.Orange;
			ConnectionColor = IndicatorColorType.Orange;

			Zones = new List<int>();
			Device = device;

			if (device.IndicatorLogic == null)
				return;

			switch (device.IndicatorLogic.IndicatorLogicType)
			{
				case IndicatorLogicType.Zone:
					IsZone = true;
					if (device.IndicatorLogic.Zones != null)
						Zones = device.IndicatorLogic.Zones;
					break;

				case IndicatorLogicType.Device:
					IsDevice = true;
					SelectedDevice = device.IndicatorLogic.Device;
					if (SelectedDevice != null)
					{
						OnColor = device.IndicatorLogic.OnColor;
						OffColor = device.IndicatorLogic.OffColor;
						FailureColor = device.IndicatorLogic.FailureColor;
						ConnectionColor = device.IndicatorLogic.ConnectionColor;
					}
					break;
			}
		}

		bool _isZone;
		public bool IsZone
		{
			get { return _isZone; }
			set
			{
				_isZone = value;
				OnPropertyChanged("IsZone");
			}
		}

		bool _isDevice;
		public bool IsDevice
		{
			get { return _isDevice; }
			set
			{
				_isDevice = value;
				OnPropertyChanged("IsDevice");
			}
		}

		public string PresenrationZones
		{
			get
			{
				string presenrationZones = "";
				for (int i = 0; i < Zones.Count; i++)
				{
					var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Zones[i]);
					if (i > 0)
						presenrationZones += ", ";
					presenrationZones += zone.PresentationName;
				}
				return presenrationZones;
			}
		}

		Device _selectedDevice;
		public Device SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		public List<IndicatorColorType> Colors
		{
			get { return Enum.GetValues(typeof(IndicatorColorType)).Cast<IndicatorColorType>().ToList(); }
		}

		IndicatorColorType _onColor;
		public IndicatorColorType OnColor
		{
			get { return _onColor; }
			set
			{
				_onColor = value;
				OnPropertyChanged("OnColor");
			}
		}

		IndicatorColorType _offColor;
		public IndicatorColorType OffColor
		{
			get { return _offColor; }
			set
			{
				_offColor = value;
				OnPropertyChanged("OffColor");
			}
		}

		IndicatorColorType _failureColor;
		public IndicatorColorType FailureColor
		{
			get { return _failureColor; }
			set
			{
				_failureColor = value;
				OnPropertyChanged("FailureColor");
			}
		}

		IndicatorColorType _connectionColor;
		public IndicatorColorType ConnectionColor
		{
			get { return _connectionColor; }
			set
			{
				_connectionColor = value;
				OnPropertyChanged("ConnectionColor");
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var indicatorZoneSelectionViewModel = new IndicatorZoneSelectionViewModel(Zones, Device);
			if (DialogService.ShowModalWindow(indicatorZoneSelectionViewModel))
			{
				Zones = indicatorZoneSelectionViewModel.Zones;
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand ShowDevicesCommand { get; private set; }
		void OnShowDevices()
		{
			var indicatorDeviceSelectionViewModel = new IndicatorDeviceSelectionViewModel();
			if (DialogService.ShowModalWindow(indicatorDeviceSelectionViewModel))
				SelectedDevice = indicatorDeviceSelectionViewModel.SelectedDevice.Device;
		}

		public RelayCommand ResetDeviceCommand { get; private set; }
		void OnResetDevice()
		{
			SelectedDevice = null;
		}

		protected override bool Save()
		{
			Device.IndicatorLogic = new IndicatorLogic();

			if (IsZone)
			{
				Device.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Zone;
				Device.IndicatorLogic.Zones = Zones;
			}
			else if (IsDevice)
			{
				Device.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Device;
				Device.IndicatorLogic.Device = SelectedDevice;
				Device.IndicatorLogic.DeviceUID = (SelectedDevice == null) ? Guid.Empty : SelectedDevice.UID;
				Device.IndicatorLogic.OnColor = OnColor;
				Device.IndicatorLogic.OffColor = OffColor;
				Device.IndicatorLogic.FailureColor = FailureColor;
				Device.IndicatorLogic.ConnectionColor = ConnectionColor;
			}
			return base.Save();
		}
	}
}