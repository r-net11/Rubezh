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
		List<Guid> Zones;

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

			Zones = new List<Guid>();
			Device = device;

			if (device.IndicatorLogic == null)
				return;

			if (device.IndicatorLogic.Device == null && device.IndicatorLogic.Zones.Count == 0)
			{
				var intLogicType = Infrastructure.Common.RegistrySettingsHelper.GetInt("FireAdministrator.Indicator.IndicatorLogicType");
				if (intLogicType != 0)
				{
					device.IndicatorLogic.IndicatorLogicType = (IndicatorLogicType)intLogicType;
				}
			}

			switch (device.IndicatorLogic.IndicatorLogicType)
			{
				case IndicatorLogicType.Zone:
					IsZone = true;
					if (device.IndicatorLogic.ZoneUIDs != null)
						Zones = device.IndicatorLogic.ZoneUIDs;
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
				OnPropertyChanged(() => IsZone);
			}
		}

		bool _isDevice;
		public bool IsDevice
		{
			get { return _isDevice; }
			set
			{
				_isDevice = value;
				OnPropertyChanged(() => IsDevice);
			}
		}

		public string PresenrationZones
		{
			get
			{
				string presenrationZones = "";
				for (int i = 0; i < Zones.Count; i++)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == Zones[i]);
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
				OnPropertyChanged(() => SelectedDevice);
				OnPropertyChanged(() => CanEditColors);
			}
		}

		public bool CanEditColors
		{
			get
			{
				return SelectedDevice != null && SelectedDevice.Driver.DriverType != DriverType.PumpStation && SelectedDevice.Parent.Driver.DriverType != DriverType.PumpStation;
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
				OnPropertyChanged(() => OnColor);
			}
		}

		IndicatorColorType _offColor;
		public IndicatorColorType OffColor
		{
			get { return _offColor; }
			set
			{
				_offColor = value;
				OnPropertyChanged(() => OffColor);
			}
		}

		IndicatorColorType _failureColor;
		public IndicatorColorType FailureColor
		{
			get { return _failureColor; }
			set
			{
				_failureColor = value;
				OnPropertyChanged(() => FailureColor);
			}
		}

		IndicatorColorType _connectionColor;
		public IndicatorColorType ConnectionColor
		{
			get { return _connectionColor; }
			set
			{
				_connectionColor = value;
				OnPropertyChanged(() => ConnectionColor);
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectionViewModel = new ZonesSelectionViewModel(Device, Zones);
			if (DialogService.ShowModalWindow(zonesSelectionViewModel))
			{
				Zones = zonesSelectionViewModel.Zones;
				OnPropertyChanged(() => PresenrationZones);
			}
		}

		public RelayCommand ShowDevicesCommand { get; private set; }
		void OnShowDevices()
		{
			var indicatorDeviceSelectionViewModel = new IndicatorDeviceSelectionViewModel(Device, SelectedDevice);
			if (DialogService.ShowModalWindow(indicatorDeviceSelectionViewModel))
			{
				SelectedDevice = indicatorDeviceSelectionViewModel.SelectedDevice;
			}
		}

		public RelayCommand ResetDeviceCommand { get; private set; }
		void OnResetDevice()
		{
			SelectedDevice = null;
		}

		protected override bool Save()
		{
			var indicatorLogic = new IndicatorLogic();
			if (IsZone)
			{
				indicatorLogic.IndicatorLogicType = IndicatorLogicType.Zone;
				indicatorLogic.ZoneUIDs = Zones;
			}
			else if (IsDevice)
			{
				indicatorLogic.IndicatorLogicType = IndicatorLogicType.Device;
				indicatorLogic.Device = SelectedDevice;
				indicatorLogic.DeviceUID = (SelectedDevice == null) ? Guid.Empty : SelectedDevice.UID;
				indicatorLogic.OnColor = OnColor;
				indicatorLogic.OffColor = OffColor;
				indicatorLogic.FailureColor = FailureColor;
				indicatorLogic.ConnectionColor = ConnectionColor;

				if (indicatorLogic.Device != null && indicatorLogic.Device.Driver.IsPump())
				{
					if (Device.IntAddress > 10)
					{
						MessageBoxService.ShowError("Размещать устройства данного типа можно только на индикаторах с адресом меньше 11");
						return false;
					}
					if (!MessageBoxService.ShowQuestion("Разместить устройство на нескольких индикаторах?"))
						return false;

					for (int i = 1; i < 5; i++)
					{
						var nextIndicatorDevice = Device.Parent.Children[Device.IntAddress + i * 10 - 1];
						var nextIndicatorLogic = new IndicatorLogic()
						{
							IndicatorLogicType = IndicatorLogicType.Device,
							Device = Device,
							DeviceUID = Device.UID
						};
						FiresecManager.FiresecConfiguration.SetIndicatorLogic(nextIndicatorDevice, nextIndicatorLogic);
					}
				}
			}
			FiresecManager.FiresecConfiguration.SetIndicatorLogic(Device, indicatorLogic);
			ValidateMissingPumpDevices();
			RegistrySettingsHelper.SetInt("FireAdministrator.Indicator.IndicatorLogicType", (int)indicatorLogic.IndicatorLogicType);
			return base.Save();
		}

		void ValidateMissingPumpDevices()
		{
			foreach (var device in Device.Parent.Children)
			{
				if (device.IndicatorLogic.Device != null && device.IndicatorLogic.Device.Driver.DriverType == DriverType.Indicator && device.IntAddress > 10 &&
					Device.Parent.Children[device.IntAddress % 10 - 1].IndicatorLogic.Device == null)
				{
					device.IndicatorLogic.Device = null;
					device.IndicatorLogic.DeviceUID = Guid.Empty;
					device.OnChanged();
				}
			}
		}
	}
}