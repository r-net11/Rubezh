﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using GKModule.Plans.Designer;

namespace GKModule.ViewModels
{
	public partial class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(GKDevice device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			SelectCommand = new RelayCommand(OnSelect, CanSelect);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowLogicCommand = new RelayCommand(OnShowLogic, CanShowLogic);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic, CanShowZoneOrLogic);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowDoorCommand = new RelayCommand(OnShowDoor);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(Device);

			AvailvableDrivers = new ObservableCollection<GKDriver>();
			UpdateDriver();
			InitializeParamsCommands();
			Device.Changed += OnChanged;
		}

		public void CheckShleif()
		{
			if (Device != null && Device.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif && (Device.IntAddress % 2 == 0))
			{
				var kauDevice = Device.Parent;
				if (kauDevice == null)
					return;
				var alsProperty = kauDevice.Properties.FirstOrDefault(x => x.Name == "als" + (Device.IntAddress - 1).ToString() + (Device.IntAddress).ToString());
				IsDisabled = alsProperty != null && alsProperty.Value != 0;
			}
		}

		public bool IsDisabled
		{
			get { return Device.IsDisabled; }
			set
			{
				Device.IsDisabled = value;
				OnPropertyChanged(() => IsDisabled);
			}
		}

		void OnChanged()
		{
			Children.ForEach(x => x.CheckShleif());
			OnPropertyChanged(() => PresentationAddress);
			OnPropertyChanged(() => IsInDoor);
		}

		public void UpdateProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged(() => PropertiesViewModel);
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
			OnPropertyChanged(() => Description);
		}

		public bool IsInDoor
		{
			get
			{
				return GKManager.Doors.Any(x => x.EnterDeviceUID == Device.UID || x.ExitDeviceUID == Device.UID
					|| x.EnterButtonUID == Device.UID || x.ExitButtonUID == Device.UID
					|| x.LockDeviceUID == Device.UID || x.LockDeviceExitUID == Device.UID
					|| x.LockControlDeviceUID == Device.UID || x.LockControlDeviceExitUID == Device.UID);
			}
		}

		public string Address
		{
			get { return Device.Address; }
			set
			{
				Device.SetAddress(value);
				if (Driver.IsGroupDevice)
				{
					foreach (var deviceViewModel in Children)
					{
						deviceViewModel.OnPropertyChanged("Ip");
						deviceViewModel.OnPropertyChanged(() => PresentationAddress);
					}
				}
				OnPropertyChanged("Ip");
				OnPropertyChanged(() => PresentationAddress);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string PresentationAddress
		{
			get { return Device.PresentationAddress; }
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				Device.IsNotUsed = !value;
				GKManager.ChangeLogic(Device, new GKLogic());
				OnPropertyChanged(() => IsUsed);
				OnPropertyChanged(() => ShowOnPlan);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			NewDeviceViewModelBase newDeviceViewModel;
			if (Device.IsConnectedToKAU)
				newDeviceViewModel = new RSR2NewDeviceViewModel(this);
			else
				newDeviceViewModel = new NewDeviceViewModel(this);

			if (newDeviceViewModel.Drivers.Count == 1)
			{
				newDeviceViewModel.SaveCommand.Execute();
				foreach (var addedDevice in newDeviceViewModel.AddedDevices)
				{
					DevicesViewModel.Current.AllDevices.Add(addedDevice);
				}
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevices.LastOrDefault();
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
				return;
			}
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				foreach (var addedDevice in newDeviceViewModel.AddedDevices)
				{
					DevicesViewModel.Current.AllDevices.Add(addedDevice);
					foreach (var childDeviceViewModel in addedDevice.Children)
					{
						DevicesViewModel.Current.AllDevices.Add(childDeviceViewModel);
					}
				}
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevices.LastOrDefault();
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public bool CanAdd()
		{
			if (IsDisabled || GetAllParents().Any(x => x.IsDisabled))
				return false;
			if (Device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU))
			{
				if (Device.DriverType == GKDriverType.KAUIndicator)
					return false;
				if (Device.Parent != null && Device.Parent.Driver.IsGroupDevice)
					return false;
				return true;
			}
			if (Driver.Children.Count > 0)
				return true;
			return false;
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			if (GetAllParents().Any(x => x.IsDisabled))
				return false;
			return ((Parent != null) && (Parent.AddCommand.CanExecute(null)));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Remove(true);
			if (Device.KAUParent != null)
				GKManager.RebuildRSR2Addresses(Device.KAUParent);
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.IsGroupDevice);
		}
		public void Remove(bool updateParameters)
		{
			var allDevices = Device.AllChildrenAndSelf;
			allDevices.ForEach(device => device.OnChanged());
			using (var cache = new ElementDeviceUpdater())
				cache.ResetDevices(allDevices);

			var parent = Parent;
			if (parent != null)
			{
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();
				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var childDeviceViewModel in GetAllChildren(true))
				{
					DevicesViewModel.Current.AllDevices.Remove(childDeviceViewModel);
				}
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			var devicesOnShleifViewModel = new DevicesOnShleifViewModel(Device);
			DialogService.ShowModalWindow(devicesOnShleifViewModel);
		}
		bool CanSelect()
		{
			return Driver.DriverType == GKDriverType.RSR2_KAU_Shleif || Driver.DriverType == GKDriverType.RSR2_MVP_Part;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
		bool CanShowProperties()
		{
			return false;
		}

		bool _isLogicGrayed;
		public bool IsLogicGrayed
		{
			get { return _isLogicGrayed; }
			set
			{
				_isLogicGrayed = value;
				OnPropertyChanged(() => IsLogicGrayed);
			}
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsDeviceOnShleif || Device.Children.Count > 0); }
		}
		public VisualizationState VisualizationState
		{
			get { return Driver != null && Driver.IsPlaceable ? (IsOnPlan ? (Device.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent) : VisualizationState.Prohibit; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			IsSelected = true;
			var plansElement = new ElementGKDevice
				{
					DeviceUID = Device.UID
				};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}
		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}

		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }
		private UIElement OnCreateDragVisual(IDataObject dataObject)
		{
			ServiceFactory.Layout.SetRightPanelVisible(true);
			var brush = PictureCacheSource.GKDevicePicture.GetBrush(Device);
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
			if (Device.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Device.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		private void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Device.AllowMultipleVizualization = isAllow;
			Update();
		}
		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Device.AllowMultipleVizualization != isAllow;
		}

		#region Zone and Logic
		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			var hasOnNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOnNow_InManual);
			var hasOffNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOffNow_InManual);
			var hasStopClause = Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV;
			var logicViewModel = new LogicViewModel(Device, Device.Logic, true, hasOnNowClause, hasOffNowClause, hasStopClause);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.ChangeLogic(Device, logicViewModel.GetModel());
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowLogic()
		{
			return Driver.HasLogic && !Device.IsNotUsed;
		}

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			IsSelected = true;

			if (CanShowLogic())
				OnShowLogic();
		}
		bool CanShowZoneOrLogic()
		{
			return (CanShowLogic());
		}

		public bool IsZoneOrLogic
		{
			get { return CanShowZoneOrLogic(); }
		}
		#endregion

		#region Driver
		public GKDriver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.DriverType != value.DriverType)
				{
					GKManager.ChangeDriver(Device, value);
					Nodes.Clear();
					foreach (var childDevice in Device.Children)
					{
						DevicesViewModel.Current.AddDevice(childDevice, this);
					}
					OnPropertyChanged(() => Device);
					OnPropertyChanged(() => Driver);
					OnPropertyChanged(() => Device);
					OnPropertyChanged(() => Children);
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged(() => PropertiesViewModel);
					if (Device.KAUParent != null)
						GKManager.RebuildRSR2Addresses(Device.KAUParent);
					GKManager.DeviceConfiguration.Update();
					Update();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		public ObservableCollection<GKDriver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				foreach (var driverType in Device.Parent.Driver.Children)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType);
					if (CanDriverBeChanged(driver))
					{
						AvailvableDrivers.Add(driver);
					}
				}
			}
		}

		public bool CanDriverBeChanged(GKDriver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (driver.DriverType == GKDriverType.RSR2_MVP_Part)
				return false;

			if (driver.IsAutoCreate)
				return false;
			if (Device.Parent.Driver.IsGroupDevice)
				return false;
			return driver.IsDeviceOnShleif;
		}

		public bool CanChangeDriver
		{
			get { return CanDriverBeChanged(Device.Driver); }
		}
		#endregion

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactoryBase.Events.GetEvent<ShowGKDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public bool IsBold { get; set; }

		public string DoorName
		{
			get
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.EnterDeviceUID == Device.UID || x.ExitDeviceUID == Device.UID
					|| x.EnterButtonUID == Device.UID || x.ExitButtonUID == Device.UID
					|| x.LockDeviceUID == Device.UID || x.LockDeviceExitUID == Device.UID
					|| x.LockControlDeviceUID == Device.UID || x.LockControlDeviceExitUID == Device.UID);
				if (door != null)
					return door.Name;
				return null;
			}
		}

		public RelayCommand ShowDoorCommand { get; private set; }
		void OnShowDoor()
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.EnterDeviceUID == Device.UID || x.ExitDeviceUID == Device.UID
					|| x.EnterButtonUID == Device.UID || x.ExitButtonUID == Device.UID
					|| x.LockDeviceUID == Device.UID || x.LockDeviceExitUID == Device.UID
					|| x.LockControlDeviceUID == Device.UID || x.LockControlDeviceExitUID == Device.UID);
			if (door != null)
				ServiceFactoryBase.Events.GetEvent<ShowGKDoorEvent>().Publish(door.UID);
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }

		#region OPC
		public bool CanOPCUsed
		{
			get { return Device.Driver.IsPlaceable; }
		}

		public bool IsOPCUsed
		{
			get { return Device.IsOPCUsed; }
			set
			{
				Device.IsOPCUsed = value;
				OnPropertyChanged(() => IsOPCUsed);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		#endregion
	}
}