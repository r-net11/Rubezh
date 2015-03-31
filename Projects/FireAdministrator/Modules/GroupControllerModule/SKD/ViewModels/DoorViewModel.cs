using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Windows;
using DeviceControls;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Painters;
using System.Windows.Shapes;
using FiresecAPI.Models;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; set; }

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			ChangeEnterDeviceCommand = new RelayCommand(OnChangeEnterDevice);
			ChangeExitDeviceCommand = new RelayCommand(OnChangeExitDevice);
			ChangeLockDeviceCommand = new RelayCommand(OnChangeLockDevice);
			ChangeLockControlDeviceCommand = new RelayCommand(OnChangeLockControlDevice);
			ChangeEnterZoneCommand = new RelayCommand(OnChangeEnterZone);
			ChangeExitZoneCommand = new RelayCommand(OnChangeExitZone);
			ChangeOpenRegimeLogicCommand = new RelayCommand(OnChangeOpenRegimeLogic);
			ChangeNormRegimeLogicCommand = new RelayCommand(OnChangeNormRegimeLogic);
			ChangeCloseRegimeLogicCommand = new RelayCommand(OnChangeCloseRegimeLogic);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			Update();
			door.Changed += () => Update(Door);			
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Door.Description; }
			set
			{
				Door.Description = value;
				Door.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Update(GKDoor door)
		{
			Door = door;
			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}
		public void Update()
		{
			UpdateDoorDevices();
			EnterDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.EnterDeviceUID);
			ExitDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitDeviceUID);
			LockDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceUID);
			LockControlDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceUID);
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
				if (ExitDevice != null && (Door.DoorType == GKDoorType.TwoWay && ExitDevice.DriverType != GKDriverType.RSR2_CodeReader && ExitDevice.DriverType != GKDriverType.RSR2_CardReader))
				{
					Door.ExitDeviceUID = Guid.Empty;
					ExitDevice = null;
				}
			}

			OnPropertyChanged(() => EnterDevice);
			OnPropertyChanged(() => ExitDevice);
			OnPropertyChanged(() => LockDevice);
			OnPropertyChanged(() => LockControlDevice);
			OnPropertyChanged(() => EnterZone);
			OnPropertyChanged(() => ExitZone);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
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
		public GKDevice LockDevice { get; private set; }
		public GKDevice LockControlDevice { get; private set; }
		public GKSKDZone EnterZone { get; private set; }
		public GKSKDZone ExitZone { get; private set; }

		public RelayCommand ChangeEnterDeviceCommand { get; private set; }
		void OnChangeEnterDevice()
		{
			var devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_CodeReader || x.DriverType == GKDriverType.RSR2_CardReader).ToList();
			var deviceSelectationViewModel = new DeviceSelectationViewModel(EnterDevice, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.EnterDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeExitDeviceCommand { get; private set; }
		void OnChangeExitDevice()
		{
			var devices = new List<GKDevice>();
			if (Door.DoorType == GKDoorType.OneWay)
			{
				devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1).ToList();
			}
			else
			{
				devices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_CodeReader || x.DriverType == GKDriverType.RSR2_CardReader).ToList();
			}
			var driverType = Door.DoorType == GKDoorType.OneWay ? GKDriverType.RSR2_AM_1 : GKDriverType.RSR2_CodeReader;
			var deviceSelectationViewModel = new DeviceSelectationViewModel(ExitDevice, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.ExitDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
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
				Door.LockDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
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
			var logicViewModel = new LogicViewModel(null, Door.OpenRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				Door.OpenRegimeLogic = logicViewModel.GetModel();
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
			var logicViewModel = new LogicViewModel(null, Door.NormRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				Door.NormRegimeLogic = logicViewModel.GetModel();
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
			var logicViewModel = new LogicViewModel(null, Door.CloseRegimeLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				Door.CloseRegimeLogic = logicViewModel.GetModel();
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