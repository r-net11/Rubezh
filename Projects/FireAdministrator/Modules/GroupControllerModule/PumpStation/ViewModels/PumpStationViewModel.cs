using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.Linq;
using System;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public XPumpStation PumpStation { get; set; }

		public PumpStationViewModel(XPumpStation pumpStation)
		{
			PumpStation = pumpStation;
			ChangePumpDevicesCommand = new RelayCommand(OnChangePumpDevices);
			DeletePumpDeviceCommand = new RelayCommand(OnDeleteOutputDevice);
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeForbidStartLogicCommand = new RelayCommand(OnChangeForbidStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			Update();
		}

		public void Update()
		{
			PumpDevices = new ObservableCollection<DeviceViewModel>();
			PumpStation.NSDevices = new List<XDevice>();
			foreach (var deviceUID in PumpStation.NSDeviceUIDs)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device != null)
				{
					PumpStation.NSDevices.Add(device);
					var deviceViewModel = new DeviceViewModel(device);
					PumpDevices.Add(deviceViewModel);
				}
			}
			OnPropertyChanged("PumpStation");
		}

		ObservableCollection<DeviceViewModel> _pumpDevices;
		public ObservableCollection<DeviceViewModel> PumpDevices
		{
			get { return _pumpDevices; }
			set
			{
				_pumpDevices = value;
				OnPropertyChanged("PumpDevices");
			}
		}

		DeviceViewModel _selectedPumpDevice;
		public DeviceViewModel SelectedPumpDevice
		{
			get { return _selectedPumpDevice; }
			set
			{
				_selectedPumpDevice = value;
				OnPropertyChanged("SelectedPumpDevice");
			}
		}

		public RelayCommand ChangePumpDevicesCommand { get; private set; }
		void OnChangePumpDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.Pump && (device.IntAddress <= 8 || device.IntAddress == 12))
					sourceDevices.Add(device);
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(PumpStation.NSDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				PumpStation.NSDevices = devicesSelectationViewModel.DevicesList;
				PumpStation.NSDeviceUIDs = new List<Guid>();
				foreach (var device in PumpStation.NSDevices)
				{
					PumpStation.NSDeviceUIDs.Add(device.UID);
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeletePumpDeviceCommand { get; private set; }
		void OnDeleteOutputDevice()
		{
			if (SelectedPumpDevice == null)
				return;

			PumpStation.NSDeviceUIDs.Remove(SelectedPumpDevice.Device.UID);
			Update();
			ServiceFactory.SaveService.GKChanged = true;

			SelectedPumpDevice = null;
		}
		bool CanDeleteOutputDevice()
		{
			return SelectedPumpDevice != null;
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, PumpStation.StartLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				PumpStation.StartLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StartPumpStationPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPumpStationPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.StartLogic); }
		}

		public RelayCommand ChangeForbidStartLogicCommand { get; private set; }
		void OnChangeForbidStartLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, PumpStation.ForbidLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				PumpStation.ForbidLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("ForbidStartPumpStationPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string ForbidStartPumpStationPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.ForbidLogic); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, PumpStation.StopLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				PumpStation.StopLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StopPumpStationPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPumpStationPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.StopLogic); }
		}
	}
}