﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public GKPumpStation PumpStation { get; private set; }

		public PumpStationViewModel(GKPumpStation pumpStation)
		{
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			ChangeAutomaticOffLogicCommand = new RelayCommand(OnChangeAutomaticOffLogic);

			PumpStation = pumpStation;
			PumpStation.Changed += Update;
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
					device.Logic = new GKLogic();
					PumpStation.NSDevices.Add(device);
					var deviceViewModel = new DeviceViewModel(device);
					PumpDevices.Add(deviceViewModel);
					device.OnChanged();
				}
			}
			OnPropertyChanged(() => PumpStation);
			OnPropertyChanged(() => StartPresentationName);
			OnPropertyChanged(() => StopPresentationName);
			OnPropertyChanged(() => AutomaticOffPresentationName);
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
				if (device.Driver.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.Driver.DriverType == GKDriverType.RSR2_Bush_Jokey || device.Driver.DriverType == GKDriverType.RSR2_Bush_Fire)
				{
					sourceDevices.Add(device);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(PumpStation.NSDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				GKManager.ChangePumpDevices(PumpStation, devicesSelectationViewModel.DevicesList);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void DeletePumpDevice()
		{
			if (SelectedPumpDevice != null)
			{
				var device = SelectedPumpDevice.Device;
				PumpStation.NSDeviceUIDs.Remove(SelectedPumpDevice.Device.UID);
				SelectedPumpDevice.Device.OutputDependentElements.Remove(PumpStation);
				PumpStation.InputDependentElements.Remove(SelectedPumpDevice.Device);
				Update();
				device.NSLogic = new GKLogic();
				device.OnChanged();
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
			var logicViewModel = new LogicViewModel(PumpStation, PumpStation.StartLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.StartLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StartPresentationName);
				PumpStation.ChangedLogic();
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
			var logicViewModel = new LogicViewModel(PumpStation, PumpStation.StopLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.StopLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StopPresentationName);
				PumpStation.ChangedLogic();
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
			var logicViewModel = new LogicViewModel(PumpStation, PumpStation.AutomaticOffLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				PumpStation.AutomaticOffLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => AutomaticOffPresentationName);
				PumpStation.ChangedLogic();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string AutomaticOffPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.AutomaticOffLogic); }
		}
	}
}