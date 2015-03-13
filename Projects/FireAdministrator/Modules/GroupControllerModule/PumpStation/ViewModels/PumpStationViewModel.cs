﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public GKPumpStation PumpStation { get; set; }

		public PumpStationViewModel(GKPumpStation pumpStation)
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
			PumpStation.NSDevices = new List<GKDevice>();
			foreach (var deviceUID in PumpStation.NSDeviceUIDs)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device != null)
				{
					PumpStation.NSDevices.Add(device);
					var deviceViewModel = new DeviceViewModel(device);
					PumpDevices.Add(deviceViewModel);
				}
			}
			OnPropertyChanged(() => PumpStation);
		}

		ObservableCollection<DeviceViewModel> _pumpDevices;
		public ObservableCollection<DeviceViewModel> PumpDevices
		{
			get { return _pumpDevices; }
			set
			{
				_pumpDevices = value;
				OnPropertyChanged(() => PumpDevices);
			}
		}

		DeviceViewModel _selectedPumpDevice;
		public DeviceViewModel SelectedPumpDevice
		{
			get { return _selectedPumpDevice; }
			set
			{
				_selectedPumpDevice = value;
				OnPropertyChanged(() => SelectedPumpDevice);
			}
		}

		public void ChangePumpDevices()
		{
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.Driver.DriverType == GKDriverType.RSR2_Bush_Jokey || device.Driver.DriverType == GKDriverType.RSR2_Bush_Fire || device.Driver.DriverType == GKDriverType.RSR2_Bush_Shuv)
				{
					sourceDevices.Add(device);
				}
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
			var logicViewModel = new LogicViewModel(null, PumpStation.StartLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.StartLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StartPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.StartLogic); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var logicViewModel = new LogicViewModel(null, PumpStation.StopLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.StopLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StopPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.StopLogic); }
		}

		public RelayCommand ChangeAutomaticOffLogicCommand { get; private set; }
		void OnChangeAutomaticOffLogic()
		{
			var logicViewModel = new LogicViewModel(null, PumpStation.AutomaticOffLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.AutomaticOffLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => AutomaticOffPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string AutomaticOffPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.AutomaticOffLogic); }
		}
	}
}