using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public GKMPT MPT { get; private set; }

		public MPTViewModel(GKMPT mpt)
		{
			MPT = mpt;
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			ChangeStopLogicCommand = new RelayCommand(OnChangeStopLogic);
			ChangeSuspendLogicCommand = new RelayCommand(OnChangeSuspendLogic);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(() => OnEdit(), () => SelectedDevice != null);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedDevice != null);
			EditPropertiesCommand = new RelayCommand(OnEditProperties, CanEditProperties);

			MPT.Changed += Update;
			Devices = new ObservableCollection<MPTDeviceViewModel>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				var deviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
			Update();
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
				SelectedDevice = mptDeviceViewModel;
				if (OnEdit())
				{
					MPT.MPTDevices.Add(mptDevice);
					Devices.Add(mptDeviceViewModel);
					ServiceFactory.SaveService.GKChanged = true;
					MPT.ChangedLogic();
				}
			}
		}

		public RelayCommand EditCommand { get; private set; }
		bool OnEdit()
		{
			var devices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (GKMPTDevice.GetAvailableMPTDriverTypes(SelectedDevice.MPTDeviceType).Any(x => device.DriverType == x))
					if (!device.IsInMPT || device.Driver.IsCardReaderOrCodeReader)
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
				MPT.ChangedLogic();
				ServiceFactory.SaveService.GKChanged = true;
				return selectedDevice != null;
			}
			return false;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ChangeIsInMPT(SelectedDevice.MPTDevice.Device, false);
			MPT.MPTDevices.Remove(SelectedDevice.MPTDevice);
			Devices.Remove(SelectedDevice);
			MPT.ChangedLogic();
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand EditPropertiesCommand { get; private set; }
		void OnEditProperties()
		{
			var mptDevicePropertiesViewModel = new MPTDevicePropertiesViewModel(SelectedDevice.Device, true);
			if (DialogService.ShowModalWindow(mptDevicePropertiesViewModel))
			{
				SelectedDevice.MPTDevicePropertiesViewModel.Update(false);
				ChangeIsInMPT(SelectedDevice.MPTDevice.Device, true);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanEditProperties()
		{
			return SelectedDevice != null && SelectedDevice.Device != null;
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
			this.VisualizationState = MPT.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (MPT.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			Devices = new ObservableCollection<MPTDeviceViewModel>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				var deviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
			OnPropertyChanged(() => MPT);
			OnPropertyChanged(() => Devices);
			OnPropertyChanged(() => StartPresentationName);
			OnPropertyChanged(() => StopPresentationName);
			OnPropertyChanged(() => SuspendPresentationName);
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var logicViewModel = new LogicViewModel(MPT, MPT.MptLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetMPTLogic(MPT, logicViewModel.GetModel());
				OnPropertyChanged(() => StartPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.OnClausesGroup); }
		}

		public RelayCommand ChangeStopLogicCommand { get; private set; }
		void OnChangeStopLogic()
		{
			var logicViewModel = new LogicViewModel(MPT, MPT.MptLogic, true, hasOnClause: false);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetMPTLogic(MPT, logicViewModel.GetModel());
				OnPropertyChanged(() => StopPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StopPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.OffClausesGroup); }
		}

		public RelayCommand ChangeSuspendLogicCommand { get; private set; }
		void OnChangeSuspendLogic()
		{
			var logicViewModel = new LogicViewModel(MPT, MPT.MptLogic, hasStopClause: true, hasOnClause: false);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetMPTLogic(MPT, logicViewModel.GetModel());
				OnPropertyChanged(() => SuspendPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string SuspendPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.StopClausesGroup); }
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

		private VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				this._visualizationState = value;
				base.OnPropertyChanged(() => this.VisualizationState);
			}
		}
	}
}