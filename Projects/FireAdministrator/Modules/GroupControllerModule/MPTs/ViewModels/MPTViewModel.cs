using System;
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
	public class MPTViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public GKMPT MPT { get; set; }

		public MPTViewModel(GKMPT mpt)
		{
			MPT = mpt;
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			ChangeSuspendLogicCommand = new RelayCommand(OnChangeSuspendLogic);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditPropertiesCommand = new RelayCommand(OnEditProperties, CanEditProperties);

			Devices = new ObservableCollection<MPTDeviceViewModel>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				var deviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		ObservableCollection<MPTDeviceViewModel> _devices;
		public ObservableCollection<MPTDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		MPTDeviceViewModel _selectedDevice;
		public MPTDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var mptDeviceTypeSelectationViewModel = new MPTDeviceTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(mptDeviceTypeSelectationViewModel))
			{
				var mptDevice = new GKMPTDevice();
				mptDevice.MPTDeviceType = mptDeviceTypeSelectationViewModel.SelectedMPTDeviceType.MPTDeviceType;

				var mptDeviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(mptDeviceViewModel);
				SelectedDevice = mptDeviceViewModel;
				MPT.MPTDevices.Add(mptDevice);

				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var devices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (GKMPTDevice.GetAvailableMPTDriverTypes(SelectedDevice.MPTDeviceType).Any(x => device.DriverType == x))
					if (!device.IsInMPT || device.DriverType == GKDriverType.RSR2_CardReader || device.DriverType == GKDriverType.RSR2_CodeReader)
						devices.Add(device);
			}

			var deviceSelectationViewModel = new DeviceSelectationViewModel(SelectedDevice.MPTDevice.Device, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (SelectedDevice.MPTDevice.Device != null)
				{
					ChangeIsInMPT(SelectedDevice.MPTDevice.Device, false);
				}

				var selectedDevice = deviceSelectationViewModel.SelectedDevice;
				SelectedDevice.MPTDevice.Device = selectedDevice;
				SelectedDevice.MPTDevice.DeviceUID = selectedDevice != null ? selectedDevice.UID : Guid.Empty;
				GKManager.DeviceConfiguration.SetMPTDefaultProperty(selectedDevice, SelectedDevice.MPTDeviceType);
				GKManager.DeviceConfiguration.SetIsMPT(SelectedDevice.MPTDevice);
				SelectedDevice.Device = selectedDevice;
				ChangeIsInMPT(SelectedDevice.MPTDevice.Device, true);
				SelectedDevice.MPTDevicePropertiesViewModel = new MPTDevicePropertiesViewModel(selectedDevice, false);

				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedDevice != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ChangeIsInMPT(SelectedDevice.MPTDevice.Device, false);
			MPT.MPTDevices.Remove(SelectedDevice.MPTDevice);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedDevice != null;
		}

		public RelayCommand EditPropertiesCommand { get; private set; }
		void OnEditProperties()
		{
			var mptDevicePropertiesViewModel = new MPTDevicePropertiesViewModel(SelectedDevice.Device, true);
			if(DialogService.ShowModalWindow(mptDevicePropertiesViewModel))
			{
				SelectedDevice.MPTDevicePropertiesViewModel.Update(false);
				ChangeIsInMPT(SelectedDevice.MPTDevice.Device, true);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanEditProperties()
		{
			return SelectedDevice != null && SelectedDevice.Device!= null;
		}

		public static void ChangeIsInMPT(GKDevice device, bool isInMPT)
		{
			if (device != null)
			{
				device.IsInMPT = isInMPT;
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == device.UID);
				if (deviceViewModel != null)
				{
					deviceViewModel.UpdateProperties();
				}
				device.OnChanged();
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => MPT);
			_visualizetionState = MPT.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (MPT.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => MPT);
		}

		public void Update(GKMPT mpt)
		{
			MPT = mpt;
			Update();
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var logicViewModel = new LogicViewModel(null, MPT.StartLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				MPT.StartLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StartPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.StartLogic); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var logicViewModel = new LogicViewModel(null, MPT.StopLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				MPT.StopLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => StopPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.StopLogic); }
		}

		public RelayCommand ChangeSuspendLogicCommand { get; private set; }
		void OnChangeSuspendLogic()
		{
			var logicViewModel = new LogicViewModel(null, MPT.SuspendLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				MPT.SuspendLogic = logicViewModel.GetModel();
				OnPropertyChanged(() => SuspendPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string SuspendPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.SuspendLogic); }
		}

		public int Delay
		{
			get { return MPT.Delay; }
			set
			{
				MPT.Delay = value;
				OnPropertyChanged(() => Delay);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
	}
}