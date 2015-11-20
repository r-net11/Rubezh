using System;
using System.Linq;
using System.Collections.Generic;
using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using RubezhClient;
using System.Windows;
using DeviceControls;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Painters;
using System.Windows.Shapes;
using RubezhAPI.Models;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; private set; }

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			ChangeEnterDeviceCommand = new RelayCommand(OnChangeEnterDevice);
			ChangeExitDeviceCommand = new RelayCommand(OnChangeExitDevice);
			ChangeEnterButtonCommand = new RelayCommand(OnChangeEnterButton);
			ChangeExitButtonCommand = new RelayCommand(OnChangeExitButton);
			ChangeLockDeviceCommand = new RelayCommand(OnChangeLockDevice);
			ChangeLockDeviceExitCommand = new RelayCommand(OnChangeLockDeviceExit);
			ChangeLockControlDeviceCommand = new RelayCommand(OnChangeLockControlDevice);
			ChangeLockControlDeviceExitCommand = new RelayCommand(OnChangeLockControlDeviceExit);
			ChangeEnterZoneCommand = new RelayCommand(OnChangeEnterZone);
			ChangeExitZoneCommand = new RelayCommand(OnChangeExitZone);
			ChangeOpenRegimeLogicCommand = new RelayCommand(OnChangeOpenRegimeLogic);
			ChangeNormRegimeLogicCommand = new RelayCommand(OnChangeNormRegimeLogic);
			ChangeCloseRegimeLogicCommand = new RelayCommand(OnChangeCloseRegimeLogic);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			Update();
			door.Changed += () => Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Door);
			UpdateDoorDevices();
			EnterDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.EnterDeviceUID);
			ExitDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitDeviceUID);
			EnterButton = GKManager.Devices.FirstOrDefault(x => x.UID == Door.EnterButtonUID);
			ExitButton = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitButtonUID);
			LockDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceUID);
			LockDeviceExit = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceExitUID);
			LockControlDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceUID);
			LockControlDeviceExit = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceExitUID);
			EnterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.EnterZoneUID);
			ExitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.ExitZoneUID);

			UpdateDoorDevices();
			if (ExitDevice != null)
			{
				if (Door.DoorType == GKDoorType.OneWay && ExitDevice.DriverType != GKDriverType.RSR2_AM_1)
				{
					Door.ExitDeviceUID = Guid.Empty;
					ExitDevice = null;
				}
				if (ExitDevice != null && (Door.DoorType != GKDoorType.OneWay && ExitDevice.DriverType != GKDriverType.RSR2_CodeReader && ExitDevice.DriverType != GKDriverType.RSR2_CardReader))
				{
					Door.ExitDeviceUID = Guid.Empty;
					ExitDevice = null;
				}
			}

			OnPropertyChanged(() => EnterDevice);
			OnPropertyChanged(() => ExitDevice);
			OnPropertyChanged(() => EnterButton);
			OnPropertyChanged(() => ExitButton);
			OnPropertyChanged(() => LockDevice);
			OnPropertyChanged(() => LockDeviceExit);
			OnPropertyChanged(() => LockControlDevice);
			OnPropertyChanged(() => LockControlDeviceExit);
			OnPropertyChanged(() => EnterZone);
			OnPropertyChanged(() => ExitZone);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
			OnPropertyChanged(() => OpenRegimeLogicPresentationName);
			OnPropertyChanged(() => NormRegimeLogicPresentationName);
			OnPropertyChanged(() => CloseRegimeLogicPresentationName);
		}

		void UpdateDoorDevices()
		{
			if (EnterDevice != null)
				EnterDevice.OnChanged();
			if (ExitDevice != null)
				ExitDevice.OnChanged();
			if (LockDevice != null)
				LockDevice.OnChanged();
			if (LockControlDevice != null)
				LockControlDevice.OnChanged();
		}

		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice EnterButton { get; private set; }
		public GKDevice ExitButton { get; private set; }
		public GKDevice LockDevice { get; private set; }
		public GKDevice LockDeviceExit { get; private set; }
		public GKDevice LockControlDevice { get; private set; }
		public GKDevice LockControlDeviceExit { get; private set; }
		public GKSKDZone EnterZone { get; private set; }
		public GKSKDZone ExitZone { get; private set; }

		public RelayCommand ChangeEnterDeviceCommand { get; private set; }
		void OnChangeEnterDevice()
		{
			var devices = GKManager.Devices.Where(x => x.Driver.IsCardReaderOrCodeReader).ToList();
			var deviceSelectationViewModel = new DeviceSelectationViewModel(EnterDevice, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (Door.EnterDeviceUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
						{
							if (x.UID == Door.EnterDeviceUID)
								x.OutputDependentElements.Remove(Door);
						});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.EnterDeviceUID);
				}

				Door.EnterDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;

				if(Door.EnterDevice != null)
				{
					Door.EnterDevice.Door = null;
				}
				Door.EnterDevice = deviceSelectationViewModel.SelectedDevice;
				if (Door.EnterDevice != null)
				{
					Door.EnterDevice.Door = Door;
					Door.AddDependentElement(Door.EnterDevice);
					
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeExitDeviceCommand { get; private set; }
		void OnChangeExitDevice()
		{
			var devices = new List<GKDevice>();
			if (Door.DoorType == GKDoorType.OneWay)
				devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1).ToList();
			else
				devices = GKManager.Devices.Where(x => x.Driver.IsCardReaderOrCodeReader).ToList();
			var deviceSelectationViewModel = new DeviceSelectationViewModel(ExitDevice, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (Door.ExitDeviceUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
					{
						if (x.UID == Door.ExitDeviceUID)
							x.OutputDependentElements.Remove(Door);
					});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.ExitDeviceUID);
				}

				Door.ExitDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.ExitDevice != null)
				{
					Door.ExitDevice.Door = null;
				}
				Door.ExitDevice = deviceSelectationViewModel.SelectedDevice;
				if (Door.ExitDevice != null)
				{
					Door.ExitDevice.Door = Door;
					Door.AddDependentElement(Door.ExitDevice);
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeEnterButtonCommand { get; private set; }
		void OnChangeEnterButton()
		{
			var devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1).ToList();
			var deviceSelectationViewModel = new DeviceSelectationViewModel(EnterButton, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				//Tuple<Guid, GKDevice> tuple = new Tuple<Guid, GKDevice>(Door.EnterButtonUID, Door.EnterButton);
				//tuple = GKManager.ChangeDevice(Door, tuple, deviceSelectationViewModel.SelectedDevice);
				//GKManager.ChangeDeviceUID(ref Door.EnterButton, ref Door.EnterButtonUID, Door, deviceSelectationViewModel.SelectedDevice);
				if (Door.EnterButtonUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
					{
						if (x.UID == Door.EnterButtonUID)
							x.OutputDependentElements.Remove(Door);
					});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.EnterButtonUID);
				}
				Door.EnterButtonUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.EnterButton != null)
				{
					Door.EnterButton.Door = null;
				}
				Door.EnterButton = deviceSelectationViewModel.SelectedDevice;
				if (Door.EnterButton != null)
				{
					Door.EnterButton.Door = Door;
					Door.AddDependentElement(Door.EnterButton);
				}

				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeExitButtonCommand { get; private set; }
		void OnChangeExitButton()
		{
			var devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1).ToList();
			var deviceSelectationViewModel = new DeviceSelectationViewModel(ExitButton, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (Door.ExitButtonUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
					{
						if (x.UID == Door.ExitButtonUID)
							x.OutputDependentElements.Remove(Door);
					});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.ExitButtonUID);
				}

				Door.ExitButtonUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.ExitButton != null)
				{
					Door.ExitButton.Door = null;
				}
				Door.ExitButton = deviceSelectationViewModel.SelectedDevice;
				if (Door.ExitButton != null)
				{
					Door.ExitButton.Door = Door;
					Door.AddDependentElement(Door.ExitButton);
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockDeviceCommand { get; private set; }
		void OnChangeLockDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDevice, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_RM_1 || x.DriverType == GKDriverType.RSR2_MVK8 || x.DriverType == GKDriverType.RSR2_CodeReader || x.DriverType == GKDriverType.RSR2_CardReader));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (Door.LockDeviceUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
					{
						if (x.UID == Door.ExitButtonUID)
							x.OutputDependentElements.Remove(Door);
					});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.LockDeviceUID);
				}

				Door.LockDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.LockDevice != null)
				{
					Door.LockDevice.Door = null;
				}
				Door.LockDevice = deviceSelectationViewModel.SelectedDevice;
				if (Door.LockDevice != null)
				{
					Door.LockDevice.Door = Door;
					Door.AddDependentElement(Door.LockDevice);
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockDeviceExitCommand { get; private set; }
		void OnChangeLockDeviceExit()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDeviceExit, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_RM_1 || x.DriverType == GKDriverType.RSR2_MVK8 || x.DriverType == GKDriverType.RSR2_CodeReader || x.DriverType == GKDriverType.RSR2_CardReader));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (Door.LockDeviceExitUID != Guid.Empty)
				{
					Door.InputDependentElements.ForEach(x =>
					{
						if (x.UID == Door.LockDeviceExitUID)
							x.OutputDependentElements.Remove(Door);
					});
					Door.InputDependentElements.RemoveAll(x => x.UID == Door.LockDeviceExitUID);
				}

				Door.LockDeviceExitUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;

				if (Door.LockDeviceExit != null)
				{
					Door.LockDeviceExit.Door = null;
				}
				Door.LockDeviceExit = deviceSelectationViewModel.SelectedDevice;
				if (Door.LockDeviceExit != null)
				{
					Door.LockDeviceExit.Door = Door;
					Door.AddDependentElement(Door.LockDeviceExit);
				}

				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockControlDeviceCommand { get; private set; }
		void OnChangeLockControlDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockControlDevice, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.LockControlDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.LockDevice != null)
				{
					Door.LockDevice.Door = null;
				}
				Door.LockDevice = deviceSelectationViewModel.SelectedDevice;
				if (Door.LockDevice != null)
				{
					Door.LockDevice.Door = Door;
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockControlDeviceExitCommand { get; private set; }
		void OnChangeLockControlDeviceExit()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockControlDeviceExit, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.LockControlDeviceExitUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				if (Door.LockDeviceExit != null)
				{
					Door.LockDeviceExit.Door = null;
				}
				Door.LockDeviceExit = deviceSelectationViewModel.SelectedDevice;
				if (Door.LockDeviceExit != null)
				{
					Door.LockDeviceExit.Door = Door;
				}
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeEnterZoneCommand { get; private set; }
		void OnChangeEnterZone()
		{
			var zoneSelectationViewModel = new SKDZoneSelectationViewModel(EnterZone);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				Door.EnterZoneUID = zoneSelectationViewModel.SelectedZone != null ? zoneSelectationViewModel.SelectedZone.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeExitZoneCommand { get; private set; }
		void OnChangeExitZone()
		{
			var zoneSelectationViewModel = new SKDZoneSelectationViewModel(ExitZone);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				Door.ExitZoneUID = zoneSelectationViewModel.SelectedZone != null ? zoneSelectationViewModel.SelectedZone.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeOpenRegimeLogicCommand { get; private set; }
		void OnChangeOpenRegimeLogic()
		{
			var logicViewModel = new LogicViewModel(Door, Door.OpenRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDoorOpenRegimeLogic(Door, logicViewModel.GetModel());
				OnPropertyChanged(() => OpenRegimeLogicPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string OpenRegimeLogicPresentationName
		{
			get { return GKManager.GetPresentationLogic(Door.OpenRegimeLogic); }
		}

		public RelayCommand ChangeNormRegimeLogicCommand { get; private set; }
		void OnChangeNormRegimeLogic()
		{
			var logicViewModel = new LogicViewModel(Door, Door.NormRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDoorNormRegimeLogic(Door, logicViewModel.GetModel());
				OnPropertyChanged(() => NormRegimeLogicPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string NormRegimeLogicPresentationName
		{
			get { return GKManager.GetPresentationLogic(Door.NormRegimeLogic); }
		}

		public RelayCommand ChangeCloseRegimeLogicCommand { get; private set; }
		void OnChangeCloseRegimeLogic()
		{
			var logicViewModel = new LogicViewModel(Door, Door.CloseRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDoorCloseRegimeLogic(Door, logicViewModel.GetModel());
				OnPropertyChanged(() => CloseRegimeLogicPresentationName);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string CloseRegimeLogicPresentationName
		{
			get { return GKManager.GetPresentationLogic(Door.CloseRegimeLogic); }
		}

		public bool IsOnPlan
		{
			get { return Door.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return true; }
		}
		public VisualizationState VisualizationState
		{
			get { return IsOnPlan ? (Door.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		void OnCreateDragObjectCommand(DataObject dataObject)
		{
			DoorsViewModel.Current.SelectedDoor = this;
			var plansElement = new ElementGKDoor
			{
				DoorUID = Door.UID
			};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}
		bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}

		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }
		UIElement OnCreateDragVisual(IDataObject dataObject)
		{
			ServiceFactory.Layout.SetRightPanelVisible(true);
			var brush = PictureCacheSource.DoorPicture.GetDefaultBrush();
			return new Rectangle
			{
				Fill = brush,
				Height = PainterCache.DefaultPointSize,
				Width = PainterCache.DefaultPointSize,
			};
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Door.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Door.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Door.AllowMultipleVizualization = isAllow;
			Update();
		}
		bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Door.AllowMultipleVizualization != isAllow;
		}
	}
}