using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using Common;
using DeviceControls;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
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
			ShowAsListCommand = new RelayCommand(OnShowAsList, CanShowAsList);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
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
			GenerateMPTCommand = new RelayCommand(GenerateMPTs);
			ShowAccessUserReflectionCommand = new RelayCommand(ShowAccessUserReflection);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);
			IsEdit = device.Driver.IsEditMirror;
			Device = device;
			PropertiesViewModel = new PropertiesViewModel(Device);
			AvailvableDrivers = new ObservableCollection<GKDriver>();
			UpdateDriver();
			InitializeParamsCommands();
			Device.Changed += OnChanged;
			Device.AUParametersChanged += UpdateDeviceParameterMissmatch;
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
			OnPropertyChanged(() => IsParamtersEnabled);
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
			OnPropertyChanged(() => IsParamtersEnabled);
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
			OnPropertyChanged(() => Description);
		}

		public bool IsInMPT
		{
			get { return Device != null && Device.IsInMPT; }
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

		public bool IsInPumpStation
		{
			get { return Device != null && GKManager.PumpStations.Any(x => x.InputDependentElements.Contains(Device)); }
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

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				Device.IsNotUsed = !value;
				GKManager.ChangeLogic(Device, new GKLogic());
				OnPropertyChanged(() => IsUsed);
				OnPropertyChanged(() => ShowOnPlan);
				OnPropertyChanged(() => PresentationZone);
				OnPropertyChanged(() => EditingPresentationZone);
				OnPropertyChanged(() => GuardPresentationZone);
				OnPropertyChanged(() => IsFireAndGuard);
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
				if (newDeviceViewModel.Drivers[0].DriverType == GKDriverType.GK && newDeviceViewModel.AddedDevices.Count > 0)
				{
					var gkDevice = newDeviceViewModel.AddedDevices[0];
					var gkIndicatorsGroupDevice = new DeviceViewModel(new GKDevice { Name = "Группа индикаторов", Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicatorsGroup) });
					var gkRelaysGroupDevice = new DeviceViewModel(new GKDevice { Name = "Группа реле", Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKRelaysGroup) });
					var gkIndicators = new List<DeviceViewModel>(gkDevice.Children.Where(x => x.Driver.DriverType == GKDriverType.GKIndicator));
					var gkRelays = new List<DeviceViewModel>(gkDevice.Children.Where(x => x.Driver.DriverType == GKDriverType.GKRele));
					foreach (var gkIndicator in gkIndicators)
					{
						gkIndicatorsGroupDevice.AddChild(gkIndicator);
					}
					foreach (var gkRelay in gkRelays)
					{
						gkRelaysGroupDevice.AddChild(gkRelay);
					}
					gkDevice.AddChildFirst(gkIndicatorsGroupDevice);
					gkDevice.AddChildFirst(gkRelaysGroupDevice);
				}
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
						addedDevice.IsExpanded = true;
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
			GKManager.RebuildRSR2Addresses(Device);
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.IsGroupDevice);
		}
		public void Remove(bool updateParameters)
		{
			var allDevices = Device.AllChildrenAndSelf;
			foreach (var device in allDevices)
			{
				GKManager.RemoveDevice(device);
			}
			allDevices.ForEach(device => device.OnChanged());
			using (var cache = new ElementDeviceUpdater())
				cache.ResetDevices(allDevices);
			if (updateParameters)
			{
				if (Parent != null)
				{
					Parent.Device.OnAUParametersChanged();
				}
			}

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
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorFireZone);
					GKDevice device = GKManager.AddChild(Device, null, driver, 0);
					device.GKMirrorItem.ZoneUIDs.Add(zone.UID);
					device.GKMirrorItem.Zones.Add(zone);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					
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
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorGuardZone);
					GKDevice device = GKManager.AddChild(Device, null, driver, (byte)0);
					device.GKMirrorItem.GuardZoneUIDs.Add(zone.UID);
					device.GKMirrorItem.GuardZones.Add(zone);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);
					
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
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorDirection);
					GKDevice device = GKManager.AddChild(Device, null, driver, (byte)0);
					device.GKMirrorItem.DiretionUIDs.Add(direction.UID);
					device.GKMirrorItem.Diretions.Add(direction);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);

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
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorDetectorsDevice);
					GKDevice device = GKManager.AddChild(Device, null, driver, (byte)0);
					device.GKMirrorItem.DeviceUIDs.Add(detectordevice.UID);
					device.GKMirrorItem.Devices.Add(detectordevice);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);

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
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorPerformersDevice);
					GKDevice device = GKManager.AddChild(Device, null, driver, (byte)0);
					device.GKMirrorItem.DeviceUIDs.Add(performerdevice.UID);
					device.GKMirrorItem.Devices.Add(performerdevice);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);

				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand GenerateMPTCommand { get; private set; }
		void GenerateMPTs()
		{
			var mPTsSelectationViewModel = new MPTsSelectationViewModel(new List<GKMPT>());
			if (DialogService.ShowModalWindow(mPTsSelectationViewModel))
			{
				foreach (var mpt in mPTsSelectationViewModel.TargetMPTs)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirrorFightFireZone);
					GKDevice device = GKManager.AddChild(Device, null, driver, (byte)0);
					device.GKMirrorItem.MPTUIDs.Add(mpt.UID);
					device.GKMirrorItem.MPTs.Add(mpt);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(device, this);
					DevicesViewModel.Current.AllDevices.Add(addedDeviceViewModel);

				}
				GKManager.RebuildRSR2Addresses(Device);
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ShowAccessUserReflectionCommand { get; private set; }
		void ShowAccessUserReflection()
		{
			var accessUserReflrctionViewModel = new ReflectionUsersViewModel(Device);
			DialogService.ShowModalWindow(accessUserReflrctionViewModel);
			ServiceFactory.SaveService.GKChanged = true;			
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
		bool CanShowProperties()
		{
			return false;
		}

		public string PresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				return GKManager.GetPresentationZoneOrLogic(Device);
			}
		}

		public string GuardPresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				if (!(Driver.HasZone && Driver.HasGuardZone))
					return null;
				return GKManager.GetPresentationGuardZone(Device);
			}
		}

		public string EditingPresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
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
			Device.OnChanged();
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
				Device.ChangedLogic();
				OnPropertyChanged(() => PresentationZone);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowLogic()
		{
			return Driver.HasLogic && !Device.IsNotUsed && !Device.IsInMPT && !IsInPumpStation;
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			if (Driver.IsAm)
			{
				var anyZonesSelectionViewModel = new AnyZonesSelectionViewModel(Device);
				if(DialogService.ShowModalWindow(anyZonesSelectionViewModel))
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
						Device.ChangedLogic();
					}
				}
			}
			OnPropertyChanged(() => PresentationZone);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanShowZones()
		{
			return (Driver.HasZone || Driver.HasGuardZone) && !Device.IsNotUsed;
		}
		bool CanShowReflection()
		{
			return Driver.HasMirror && !Device.IsNotUsed;
		}
		void OnShowReflection()
		{
			if (Driver.HasMirror)
			{
				var _reflectionview = new ReflectionViewModel(Device);
				DialogService.ShowModalWindow(_reflectionview);
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
				Device.NSLogic = logicViewModel.GetModel();
				Device.ChangedLogic();
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
					if(!GKManager.ChangeDriver(Device, value))
					{
						MessageBoxService.ShowWarning("Невозможно сменить тип устройства");
						return;
					}
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
				var door = GKManager.Doors.FirstOrDefault(x => x.EnterDeviceUID == Device.UID || x.ExitDeviceUID == Device.UID
					|| x.EnterButtonUID == Device.UID || x.ExitButtonUID == Device.UID
					|| x.LockDeviceUID == Device.UID || x.LockDeviceExitUID == Device.UID
					|| x.LockControlDeviceUID == Device.UID || x.LockControlDeviceExitUID == Device.UID);
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
			var door = GKManager.Doors.FirstOrDefault(x => x.EnterDeviceUID == Device.UID || x.ExitDeviceUID == Device.UID
					|| x.EnterButtonUID == Device.UID || x.ExitButtonUID == Device.UID
					|| x.LockDeviceUID == Device.UID || x.LockDeviceExitUID == Device.UID
					|| x.LockControlDeviceUID == Device.UID || x.LockControlDeviceExitUID == Device.UID);
			if (door != null)
				ServiceFactoryBase.Events.GetEvent<ShowGKDoorEvent>().Publish(door.UID);
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (Device != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(Device.OutDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
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
				Device.Invalidate();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanPasteLogic()
		{
			return Device.Driver.HasLogic && GKManager.LogicToCopy != null;
		}

	}
}