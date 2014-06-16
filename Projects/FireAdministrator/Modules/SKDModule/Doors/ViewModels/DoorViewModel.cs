using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public Door Door { get; private set; }
		public SKDDevice InDevice { get; private set; }
		public SKDDevice OutDevice { get; private set; }

		public DoorViewModel(Door door)
		{
			ChangeInDeviceCommand = new RelayCommand(OnChangeInDevice);
			ChangeOutDeviceCommand = new RelayCommand(OnChangeOutDevice);
			Door = door;
			Update();
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public string Description
		{
			get { return Door.Description; }
			set
			{
				Door.Description = value;
				Door.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public string InDeviceName
		{
			get
			{
				if (InDevice != null)
					return InDevice.Name;
				return "Нажмите для выбора устройства";
			}
		}

		public string OutDeviceName
		{
			get
			{
				if (OutDevice != null)
					return OutDevice.Name;
				return "Нажмите для выбора устройства";
			}
		}

		public bool IsInDeviceGrayed
		{
			get { return InDevice == null; }
		}

		public bool IsOutDeviceGrayed
		{
			get { return OutDevice == null; }
		}

		public RelayCommand ChangeInDeviceCommand { get; private set; }
		void OnChangeInDevice()
		{
			//IsSelected = true;
			var deviceSelectationViewModel = new DeviceSelectationViewModel(Door.InDeviceUID);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (deviceSelectationViewModel.SelectedDevice != null)
				{
					InDevice = deviceSelectationViewModel.SelectedDevice;
					Door.InDeviceUID = InDevice.UID;
				}
				OnPropertyChanged("InDevice");
				OnPropertyChanged("InDeviceName");
				OnPropertyChanged("IsInDeviceGrayed");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand ChangeOutDeviceCommand { get; private set; }
		void OnChangeOutDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(Door.OutDeviceUID);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (deviceSelectationViewModel.SelectedDevice != null)
				{
					OutDevice = deviceSelectationViewModel.SelectedDevice;
					Door.OutDeviceUID = OutDevice.UID;
				}
				OnPropertyChanged("OutDevice");
				OnPropertyChanged("OutDeviceName");
				OnPropertyChanged("IsOutDeviceGrayed");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(Door door)
		{
			Door = door;
			OnPropertyChanged("Door");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		public void Update()
		{
			InDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
			OutDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.OutDeviceUID);

			if (Door.PlanElementUIDs == null)
				Door.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Door.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Door.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}
	}
}