using Common;
using DeviceControls;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

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
			ShowAsListCommand = new RelayCommand(OnShowAsList, CanShowAsList);
			ShowLogicCommand = new RelayCommand(OnShowLogic, CanShowLogic);
			ShowNSLogicCommand = new RelayCommand(OnShowNSLogic, CanShowNSLogic);
			ShowZonesCommand = new RelayCommand(OnShowZones, CanShowZones);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic, CanShowZoneOrLogic);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowGuardZoneCommand = new RelayCommand(OnShowGuardZone, CanShowGuardZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowMPTCommand = new RelayCommand(OnShowMPT, CanShowMPT);
			ShowDoorCommand = new RelayCommand(OnShowDoor);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);
			GenerateZonesCommand = new RelayCommand(GenerateZones);
			GenerateGuardZonesCommand = new RelayCommand(GenerateGuardZones);
			GenerateDirectionsCommand = new RelayCommand(GenerateDirections);
			GenerateForDetectorDevicesCommand = new RelayCommand(GenerateForDetectorDevices);
			GenerateForPerformersDevicesCommand = new RelayCommand(GenerateForPerformersDevices);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			PmfUsersCommand = new RelayCommand(OnPmfUsers, CanPmfUsers);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);
			IgnoreLogicValidationCommand = new RelayCommand<bool>(OnIgnoreLogicValidationCommand, CanIgnoreLogicValidationCommand);
			IsEdit = device.Driver.IsEditMirror;
			Device = device;
			PropertiesViewModel = new PropertiesViewModel(Device);
			AvailvableDrivers = new ObservableCollection<GKDriver>();
			UpdateDriver();
			InitializeParamsCommands();
			Device.Changed += OnChanged;
			Device.PlanElementUIDsChanged += UpdateVisualizationState;
			Device.AUParametersChanged += UpdateDeviceParameterMissmatch;
			Update();
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
			OnPropertyChanged(() => PresentationZone);
			OnPropertyChanged(() => EditingPresentationZone);
			OnPropertyChanged(() => GuardPresentationZone);
			OnPropertyChanged(() => IsFireAndGuard);
			OnPropertyChanged(() => IsParametersEnabled);
			OnPropertyChanged(() => IsInDoor);
			OnPropertyChanged(() => MPTName);
			OnPropertyChanged(() => IsInMPT);
			OnPropertyChanged(() => IsInPumpStation);
			OnPropertyChanged(() => IsZoneOrLogic);
		}

		public void UpdateProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged(() => PropertiesViewModel);
			OnPropertyChanged(() => IsParametersEnabled);
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => Description);
			UpdateVisualizationState();
		}
		void UpdateVisualizationState()
		{
			VisualizationState = Driver != null && Driver.IsPlaceable ? (IsOnPlan ? (Device.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent) : VisualizationState.Prohibit;
		}

		public bool IsInMPT
		{
			get { return Device != null && Device.IsInMPT; }
		}

		public bool IsInDoor
		{
			get
			{
				return GKManager.Doors.Any(x => x.InputDependentElements.Any(y => y.UID == Device.UID));
			}
		}

		public bool IsInPumpStation
		{
			get
			{
				return Device != null && (Device.DriverType == GKDriverType.RSR2_Bush_Drenazh || Device.DriverType == GKDriverType.RSR2_Bush_Fire
				|| Device.DriverType == GKDriverType.RSR2_Bush_Jokey) && Device.OutputDependentElements.Any(x => x as GKPumpStation != null);
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

		public int SortingAddress
		{
			get
			{
				if (Device.Driver.IsKau)
					return 256 * 256 * Device.IntAddress;
				if (Device.KAUParent != null)
					return 256 * 256 * Device.KAUParent.IntAddress + Device.ShleifNo * 256 + Device.IntAddress;
				return Device.IntAddress;
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				OnPropertyChanged(() => Description);
				UpdateDescriptorName();
				Device.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string ProjectAddress
		{
			get { return Device.ProjectAddress; }
			set
			{
				Device.ProjectAddress = value;
				OnPropertyChanged(() => ProjectAddress);
				UpdateDescriptorName();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		string _descriptorName;
		public string DescriptorName
		{
			get { return _descriptorName; }
			set
			{
				_descriptorName = value;
				OnPropertyChanged(() => DescriptorName);
			}
		}

		public void UpdateDescriptorName()
		{
			if (CanUpdateDescriptorName)
			{
				DescriptorName = Device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType);
			}
		}

		public bool CanUpdateDescriptorName { get; set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel(this);

			if (newDeviceViewModel.TypedDrivers.Count == 1)
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

			if (ServiceFactory.DialogService.ShowModalWindow(newDeviceViewModel))
			{
				foreach (var addedDevice in newDeviceViewModel.AddedDevices)
				{
					DevicesViewModel.Current.AllDevices.Add(addedDevice);
					foreach (var childDeviceViewModel in addedDevice.Children)
					{
						DevicesViewModel.Current.AllDevices.Add(childDeviceViewModel);
						addedDevice.IsExpanded = true;
					}
				}
				if (DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif || DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.RSR2_MVP_Part || DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.RSR2_KDKR_Part
					|| DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.GK || DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.GKMirror || DevicesViewModel.Current.SelectedDevice.Driver.DriverType == GKDriverType.RSR2_MRK)
					DevicesViewModel.Current.SelectedDevice.IsExpanded = true;
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevices.LastOrDefault();
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public bool CanAdd()
		{
			if (IsDisabled || GetAllParents().Any(x => x.IsDisabled))
				return false;
			if (Device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU) || Device.AllParents.Any(x => x.DriverType == GKDriverType.GKMirror))
			{
				if (Device.DriverType == GKDriverType.KAUIndicator || Device.DriverType == GKDriverType.GKIndicatorsGroup || Device.DriverType == GKDriverType.GKRelaysGroup)
					return false;
				if (Device.Parent.DriverType == GKDriverType.GKRelaysGroup || Device.Parent.DriverType == GKDriverType.GKIndicatorsGroup)
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
			GKManager.RebuildRSR2Addresses(Device);
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.IsGroupDevice);
		}
		public void Remove(bool changeSelectedDevice)
		{
			var allDevices = GKManager.RemoveDevice(Device);
			foreach (var device in allDevices)
			{
				ServiceFactoryBase.Events.GetEvent<RemoveGKDeviceEvent>().Publish(device.UID);
			}

			if (Parent != null)
			{
				var parent = Parent;
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Device.OnAUParametersChanged();
				parent.Nodes.Remove(this);
				parent.Update();
				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var childDeviceViewModel in GetAllChildren(true))
				{
					DevicesViewModel.Current.AllDevices.Remove(childDeviceViewModel);
				}
				if (changeSelectedDevice)
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
			return Driver.DriverType == GKDriverType.RSR2_KAU_Shleif || Driver.DriverType == GKDriverType.RSR2_MVP_Part || Driver.DriverType == GKDriverType.RSR2_KDKR_Part;
		}

		public RelayCommand ShowAsListCommand { get; private set; }
		void OnShowAsList()
		{
			var devicesListViewModel = new DevicesListViewModel();
			DialogService.ShowModalWindow(devicesListViewModel);
		}
		bool CanShowAsList()
		{
			return true;
		}
		public RelayCommand GenerateZonesCommand { get; private set; }
		void GenerateZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(new List<GKZone>());
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				foreach (var zone in zonesSelectationViewModel.TargetZones)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.FireZonesMirror);
					GKDevice device = GKManager.AddDevice(Device, driver, 0);
					device.GKReflectionItem.ZoneUIDs.Add(zone.UID);
					device.GKReflectionItem.Zones.Add(zone);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					zone.AddDependentElement(device);
					device.AddDependentElement(zone);
				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand GenerateGuardZonesCommand { get; private set; }
		void GenerateGuardZones()
		{
			var guardZonesSelectationViewModel = new GuardZonesSelectationViewModel(new List<GKGuardZone>());
			if (DialogService.ShowModalWindow(guardZonesSelectationViewModel))
			{
				foreach (var zone in guardZonesSelectationViewModel.TargetZones)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GuardZonesMirror);
					GKDevice device = GKManager.AddDevice(Device, driver, 0);
					device.GKReflectionItem.GuardZoneUIDs.Add(zone.UID);
					device.GKReflectionItem.GuardZones.Add(zone);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					zone.AddDependentElement(device);
					device.AddDependentElement(zone);
				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand GenerateDirectionsCommand { get; private set; }
		void GenerateDirections()
		{
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(new List<GKDirection>());
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				foreach (var direction in directionsSelectationViewModel.TargetDirections)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.DirectionsMirror);
					GKDevice device = GKManager.AddDevice(Device, driver, 0);
					device.GKReflectionItem.DiretionUIDs.Add(direction.UID);
					device.GKReflectionItem.Diretions.Add(direction);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					direction.AddDependentElement(device);
					device.AddDependentElement(direction);
				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand GenerateForDetectorDevicesCommand { get; private set; }
		void GenerateForDetectorDevices()
		{
			var deviceSelectationViewMode = new DevicesSelectationViewModel(new List<GKDevice>(), GKManager.Devices.Where(x => x.Driver.HasZone).ToList());
			if (DialogService.ShowModalWindow(deviceSelectationViewMode))
			{
				foreach (var detectordevice in deviceSelectationViewMode.Devices)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.DetectorDevicesMirror);
					GKDevice device = GKManager.AddDevice(Device, driver, 0);
					device.GKReflectionItem.DeviceUIDs.Add(detectordevice.UID);
					device.GKReflectionItem.Devices.Add(detectordevice);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					detectordevice.AddDependentElement(device);
					device.AddDependentElement(detectordevice);
				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand GenerateForPerformersDevicesCommand { get; private set; }
		void GenerateForPerformersDevices()
		{
			var deviceSelectationViewMode = new DevicesSelectationViewModel(new List<GKDevice>(), GKManager.Devices.Where(x => x.Driver.IsControlDevice).ToList());
			if (DialogService.ShowModalWindow(deviceSelectationViewMode))
			{
				foreach (var performerdevice in deviceSelectationViewMode.Devices)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.ControlDevicesMirror);
					GKDevice device = GKManager.AddDevice(Device, driver, 0);
					device.GKReflectionItem.DeviceUIDs.Add(performerdevice.UID);
					device.GKReflectionItem.Devices.Add(performerdevice);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					performerdevice.AddDependentElement(device);
					device.AddDependentElement(performerdevice);
				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string PresentationZone
		{
			get
			{
				return GKManager.GetPresentationZoneOrLogic(Device);
			}
		}

		public string GuardPresentationZone
		{
			get
			{
				if (!(Driver.HasZone && Driver.HasGuardZone))
					return null;
				return GKManager.GetPresentationGuardZone(Device);
			}
		}

		public string EditingPresentationZone
		{
			get
			{
				var presentationZone = GKManager.GetPresentationZoneAndGuardZoneOrLogic(Device);
				IsLogicGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (Driver.HasZone || Driver.HasGuardZone)
						presentationZone = "Нажмите для выбора зон";
					if (Driver.HasLogic)
						presentationZone = "Нажмите для настройки логики";
					if (Driver.HasMirror)
						presentationZone = "Нажмите для настройки отражения";
					if(Device.IgnoreLogicValidation)
						presentationZone = "Запрещено";
				}
				return presentationZone;
			}
		}

		public bool IsFireAndGuard
		{
			get { return Driver.HasZone && Driver.HasGuardZone; }
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

		bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				_visualizationState = value;
				OnPropertyChanged(() => VisualizationState);
			}
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
			Device.OnChanged();
			Update();
		}
		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Device.AllowMultipleVizualization != isAllow;
		}
		public RelayCommand<bool> IgnoreLogicValidationCommand { get; private set; }
		void OnIgnoreLogicValidationCommand(bool isIgnore)
		{
			AllowLogicValidation = isIgnore;
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanIgnoreLogicValidationCommand(bool isIgnore)
		{
			return Device.IgnoreLogicValidation != isIgnore;
		}
		public bool AllowLogicValidation
		{
			get { return !Device.IgnoreLogicValidation; }
			set
			{
				Device.IgnoreLogicValidation = value;
				OnPropertyChanged(() => AllowLogicValidation);
				OnPropertyChanged(() => EditingPresentationZone);
			}
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
				GKManager.SetDeviceLogic(Device, logicViewModel.GetModel());
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowLogic()
		{
			return Driver.HasLogic && !Device.IsInMPT && !IsInPumpStation;
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			if (Driver.IsAm)
			{
				var anyZonesSelectionViewModel = new AnyZonesSelectionViewModel(Device);
				if (DialogService.ShowModalWindow(anyZonesSelectionViewModel))
					Device.ChangedLogic();
			}
			else
			{
				if (Driver.HasZone)
				{
					var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones, true);
					if (DialogService.ShowModalWindow(zonesSelectationViewModel))
					{
						GKManager.ChangeDeviceZones(Device, zonesSelectationViewModel.Zones);
						Device.ChangedLogic();
					}
				}
				if (Driver.HasGuardZone)
				{
					var guardZonesSelectationViewModel = new GuardZonesWithFuncSelectationViewModel(Device, true);
					if (DialogService.ShowModalWindow(guardZonesSelectationViewModel))
					{
						GKManager.ChangeDeviceGuardZones(Device, guardZonesSelectationViewModel.DeviceGuardZones.Select(x => x.DeviceGuardZone).ToList());
					}
				}
			}
			OnPropertyChanged(() => PresentationZone);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanShowZones()
		{
			return Driver.HasZone || Driver.HasGuardZone;
		}
		bool CanShowReflection()
		{
			return Driver.HasMirror;
		}
		void OnShowReflection()
		{
			if (Driver.HasMirror)
			{
				var _reflectionview = new MirrorViewModel(Device);
				if (DialogService.ShowModalWindow(_reflectionview))
				{
					Device.ChangedLogic();
				}
			}
			OnPropertyChanged(() => EditingPresentationZone);
			ServiceFactory.SaveService.GKChanged = true;
		}

		public bool IsEdit { get; private set; }

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			IsSelected = true;

			if (CanShowZones())
				OnShowZones();

			if (CanShowLogic())
				OnShowLogic();

			if (CanShowReflection())
				OnShowReflection();
		}
		bool CanShowZoneOrLogic()
		{
			return !Device.IsInMPT && !IsInPumpStation && (CanShowZones() || CanShowLogic() || CanShowReflection());
		}

		public bool IsZoneOrLogic
		{
			get { return CanShowZoneOrLogic(); }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			if (Driver.HasZone)
			{
				var zone = Device.Zones.FirstOrDefault();
				if (zone != null)
				{
					ServiceFactoryBase.Events.GetEvent<ShowGKZoneEvent>().Publish(zone.UID);
				}
			}
		}
		bool CanShowZone()
		{
			return Driver.HasZone && Device.Zones.Count == 1;
		}

		public RelayCommand ShowGuardZoneCommand { get; private set; }
		void OnShowGuardZone()
		{
			if (Driver.HasGuardZone)
			{
				var guardZone = Device.GuardZones.FirstOrDefault();
				if (guardZone != null)
				{
					ServiceFactoryBase.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(guardZone.UID);
				}
			}
		}
		bool CanShowGuardZone()
		{
			return Driver.HasGuardZone && Device.GuardZones.Count == 1;
		}

		public RelayCommand ShowNSLogicCommand { get; private set; }
		void OnShowNSLogic()
		{
			var logicViewModel = new LogicViewModel(Device, Device.NSLogic);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDeviceLogic(Device, logicViewModel.GetModel(), true);
				OnPropertyChanged("NSPresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowNSLogic()
		{
			return Device.DriverType == GKDriverType.RSR2_Bush_Fire;
		}

		public bool IsNSLogic
		{
			get { return CanShowNSLogic(); }
		}

		public string NSPresentationZone
		{
			get
			{
				var presentationZone = GKManager.GetPresentationLogic(Device.NSLogic);
				IsLogicGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (CanShowNSLogic())
						presentationZone = "Нажмите для настройки логики насоса";
				}
				return presentationZone;
			}
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
					Device.AllChildrenAndSelf.ForEach(x => ServiceFactoryBase.Events.GetEvent<RemoveGKDeviceEvent>().Publish(x.UID));
					var device = GKManager.ChangeDriver(Device, value);
					if (device == null)
					{
						MessageBoxService.ShowWarning("Невозможно сменить тип устройства");
						return;
					}
					Device = device;
					Device.Changed += OnChanged;
					Nodes.Clear();
					foreach (var childDevice in Device.Children)
					{
						DevicesViewModel.Current.AddDevice(childDevice, this);
					}

					OnPropertyChanged(() => Device);
					OnPropertyChanged(() => Driver);
					OnPropertyChanged(() => Children);
					OnPropertyChanged(() => EditingPresentationZone);
					OnPropertyChanged(() => GuardPresentationZone);
					OnPropertyChanged(() => IsFireAndGuard);
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged(() => PropertiesViewModel);

					GKManager.RebuildRSR2Addresses(Device);
					GKManager.DeviceConfiguration.Update();
					DevicesViewModel.Current.SelectedDevice.IsExpanded = true;
					Update();

					ServiceFactory.SaveService.GKChanged = true;
					GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
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

			if (driver.DriverType == GKDriverType.RSR2_MVP_Part || driver.DriverType == GKDriverType.RSR2_KDKR_Part)
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

		//public bool IsBold { get; set; }

		public string MPTName
		{
			get
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.MPTDevices.Any(y => y.DeviceUID == Device.UID));
				return mpt != null ? mpt.Name : null;
			}
		}

		public string DoorName
		{
			get
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.InputDependentElements.Any(y => y.UID == Device.UID));
				if (door != null)
					return door.Name;
				return null;
			}
		}

		public RelayCommand ShowMPTCommand { get; private set; }
		void OnShowMPT()
		{
			var mpt = GKManager.MPTs.FirstOrDefault(x => x.MPTDevices.Any(y => y.DeviceUID == Device.UID));
			if (mpt != null)
				ServiceFactoryBase.Events.GetEvent<ShowGKMPTEvent>().Publish(mpt.UID);
		}
		bool CanShowMPT()
		{
			return true;
		}

		public RelayCommand ShowDoorCommand { get; private set; }
		void OnShowDoor()
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.InputDependentElements.Any(y => y.UID == Device.UID));
			if (door != null)
				ServiceFactoryBase.Events.GetEvent<ShowGKDoorEvent>().Publish(door.UID);
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (Device != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(Device.OutputDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand InsertIntoCommand { get { return DevicesViewModel.Current.InsertIntoCommand; } }
		public RelayCommand CopyLogicCommand { get; private set; }
		public RelayCommand PasteLogicCommand { get; private set; }
		void OnCopyLogic()
		{
			var hasOnClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOn_InManual);
			var hasOnNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOnNow_InManual);
			var hasOffClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOff_InManual);
			var hasOffNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOffNow_InManual);
			var hasStopClause = Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV;
			GKManager.CopyLogic(Device.Logic, hasOnClause, hasOnNowClause, hasOffClause, hasOffNowClause, hasStopClause);
		}
		bool CanCopyLogic()
		{
			return Device.Driver.HasLogic;
		}

		void OnPasteLogic()
		{
			var hasOnClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOn_InManual);
			var hasOnNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOnNow_InManual);
			var hasOffClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOff_InManual);
			var hasOffNowClause = Device.Driver.AvailableCommandBits.Contains(GKStateBit.TurnOffNow_InManual);
			var hasStopClause = Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV;

			var result = GKManager.CompareLogic(new GKAdvancedLogic(hasOnClause, hasOnNowClause, hasOffClause, hasOffNowClause, hasStopClause));
			var messageBoxResult = true;
			if (!String.IsNullOrEmpty(result))
				messageBoxResult = MessageBoxService.ShowConfirmation(result, "Копировать логику?");
			if (messageBoxResult)
			{
				Device.Logic = GKManager.PasteLogic(new GKAdvancedLogic(hasOnClause, hasOnNowClause, hasOffClause, hasOffNowClause, hasStopClause));
				Device.Invalidate(GKManager.DeviceConfiguration);
				Device.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanPasteLogic()
		{
			return Device.Driver.HasLogic && GKManager.LogicToCopy != null;
		}

		public RelayCommand PmfUsersCommand { get; private set; }
		void OnPmfUsers()
		{
			DialogService.ShowModalWindow(new PmfUsersViewModel(Device));
		}
		bool CanPmfUsers()
		{
			return IsPmf;
		}
		public bool IsPmf { get { return Device.DriverType == GKDriverType.GKMirror; } }

	}
}