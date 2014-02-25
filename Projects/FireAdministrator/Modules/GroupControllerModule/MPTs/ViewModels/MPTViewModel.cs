using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public XMPT MPT { get; set; }

		public MPTViewModel(XMPT mpt)
		{
			MPT = mpt;
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			ChangeAutomaticOffLogicCommand = new RelayCommand(OnChangeAutomaticOffLogic);
			ChangeOffTableDevicesCommand = new RelayCommand(OnChangeOffTableDevices);
			ChangeOnTableDevicesCommand = new RelayCommand(OnChangeOnTableDevices);
			ChangeAutomaticTableDevicesCommand = new RelayCommand(OnChangeAutomaticTableDevices);
			ChangeSirenaDevicesCommand = new RelayCommand(OnChangeSirenaDevices);
		}

		public void Update()
		{
			OnPropertyChanged("MPT");
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, MPT.StartLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				MPT.StartLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StartPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return XManager.GetPresentationZone(MPT.StartLogic); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, MPT.StopLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				MPT.StopLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StopPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPresentationName
		{
			get { return XManager.GetPresentationZone(MPT.StopLogic); }
		}

		public RelayCommand ChangeAutomaticOffLogicCommand { get; private set; }
		void OnChangeAutomaticOffLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, MPT.AutomaticOffLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				MPT.AutomaticOffLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("AutomaticOffPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string AutomaticOffPresentationName
		{
			get { return XManager.GetPresentationZone(MPT.AutomaticOffLogic); }
		}

		public RelayCommand ChangeOffTableDevicesCommand { get; private set; }
		void OnChangeOffTableDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.RSR2_Table)
				{
					sourceDevices.Add(device);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(MPT.OffTableDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				MPT.OffTableDevices = devicesSelectationViewModel.DevicesList;
				MPT.OffTableDeviceUIDs = new List<Guid>();
				foreach (var device in MPT.OffTableDevices)
				{
					MPT.OffTableDeviceUIDs.Add(device.UID);
				}
				OnPropertyChanged("OffTableDevicesPresentationName");
			}
		}

		public string OffTableDevicesPresentationName
		{
			get { return XManager.GetCommaSeparatedDevices(MPT.OffTableDevices); }
		}

		public RelayCommand ChangeOnTableDevicesCommand { get; private set; }
		void OnChangeOnTableDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.RSR2_Table)
				{
					sourceDevices.Add(device);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(MPT.OnTableDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				MPT.OnTableDevices = devicesSelectationViewModel.DevicesList;
				MPT.OnTableDeviceUIDs = new List<Guid>();
				foreach (var device in MPT.OnTableDevices)
				{
					MPT.OnTableDeviceUIDs.Add(device.UID);
				}
				OnPropertyChanged("OnTableDevicesPresentationName");
			}
		}

		public string OnTableDevicesPresentationName
		{
			get { return XManager.GetCommaSeparatedDevices(MPT.OnTableDevices); }
		}

		public RelayCommand ChangeAutomaticTableDevicesCommand { get; private set; }
		void OnChangeAutomaticTableDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.RSR2_Table)
				{
					sourceDevices.Add(device);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(MPT.AutomaticTableDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				MPT.AutomaticTableDevices = devicesSelectationViewModel.DevicesList;
				MPT.AutomaticTableDeviceUIDs = new List<Guid>();
				foreach (var device in MPT.AutomaticTableDevices)
				{
					MPT.AutomaticTableDeviceUIDs.Add(device.UID);
				}
				OnPropertyChanged("AutomaticTableDevicesPresentationName");
			}
		}

		public string AutomaticTableDevicesPresentationName
		{
			get { return XManager.GetCommaSeparatedDevices(MPT.AutomaticTableDevices); }
		}

		public RelayCommand ChangeSirenaDevicesCommand { get; private set; }
		void OnChangeSirenaDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.RSR2_Siren)
				{
					sourceDevices.Add(device);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(MPT.SirenaDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				MPT.SirenaDevices = devicesSelectationViewModel.DevicesList;
				MPT.SirenaDeviceUIDs = new List<Guid>();
				foreach (var device in MPT.SirenaDevices)
				{
					MPT.SirenaDeviceUIDs.Add(device.UID);
				}
				OnPropertyChanged("SirenaDevicesPresentationName");
			}
		}

		public string SirenaDevicesPresentationName
		{
			get { return XManager.GetCommaSeparatedDevices(MPT.SirenaDevices); }
		}
	}
}