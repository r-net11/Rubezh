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
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			ChangeAutomaticOffLogicCommand = new RelayCommand(OnChangeAutomaticOffLogic);
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

		public void ChangePumpDevices()
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

		public void DeletePumpDevice()
		{
			if (SelectedPumpDevice != null)
			{
				PumpStation.NSDeviceUIDs.Remove(SelectedPumpDevice.Device.UID);
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
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
				OnPropertyChanged("StartPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.StartLogic); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, PumpStation.StopLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				PumpStation.StopLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StopPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.StopLogic); }
		}

		public RelayCommand ChangeAutomaticOffLogicCommand { get; private set; }
		void OnChangeAutomaticOffLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, PumpStation.AutomaticOffLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				PumpStation.AutomaticOffLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("AutomaticOffPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string AutomaticOffPresentationName
		{
			get { return XManager.GetPresentationZone(PumpStation.AutomaticOffLogic); }
		}
	}
}