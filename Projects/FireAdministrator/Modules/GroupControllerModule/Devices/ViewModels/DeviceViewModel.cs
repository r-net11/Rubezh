using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using XFiresecAPI;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public partial class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			SelectCommand = new RelayCommand(OnSelect, CanSelect);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowLogicCommand = new RelayCommand(OnShowLogic, CanShowLogic);
			ShowNSLogicCommand = new RelayCommand(OnShowNSLogic, CanShowNSLogic);
			ShowZonesCommand = new RelayCommand(OnShowZones, CanShowZones);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic, CanShowZoneOrLogic);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);

			AvailvableDrivers = new ObservableCollection<XDriver>();
			UpdateDriver();
			InitializeParamsCommands();
			device.Changed += OnChanged;
			device.AUParametersChanged += UpdateDeviceParameterMissmatch;
			device.Changed += UpdateDeviceParameterMissmatch;
		}

		void OnChanged()
		{
			OnPropertyChanged("PresentationAddress");
			OnPropertyChanged("PresentationZone");
			OnPropertyChanged("EditingPresentationZone");
		}

		public void UpdateProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged("PropertiesViewModel");
		}

		public void Update()
		{
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");
			OnPropertyChanged(() => VisualizationState);
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
						deviceViewModel.OnPropertyChanged("Address");
						deviceViewModel.OnPropertyChanged("PresentationAddress");
					}
				}
				OnPropertyChanged("Address");
				OnPropertyChanged("PresentationAddress");
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
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				Device.IsNotUsed = !value;
				XManager.ChangeDeviceLogic(Device, new XDeviceLogic());
				OnPropertyChanged("IsUsed");
				OnPropertyChanged("ShowOnPlan");
				OnPropertyChanged("PresentationZone");
				OnPropertyChanged("EditingPresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			NewDeviceViewModelBase newDeviceViewModel;
			if (Device.IsConnectedToKAURSR2OrIsKAURSR2)
				newDeviceViewModel = new RSR2NewDeviceViewModel(this);
			else
				newDeviceViewModel = new NewDeviceViewModel(this);

			if (newDeviceViewModel.Drivers.Count == 1)
			{
				newDeviceViewModel.SaveCommand.Execute();
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevice;
				Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.GKChanged = true;
				return;
			}
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public bool CanAdd()
		{
			if (Device.AllParents.Any(x => x.DriverType == XDriverType.RSR2_KAU))
				return true;
			if (Driver.Children.Count > 0)
				return true;
			if ((Driver.DriverType == XDriverType.MPT || Driver.DriverType == XDriverType.MRO_2) && Parent != null && Parent.Device.DriverType != XDriverType.MPT && Parent.Device.DriverType != XDriverType.MRO_2)
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
			return ((Parent != null) && (Parent.AddCommand.CanExecute(null)));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var allDevices = XManager.GetAllDeviceChildren(Device);
			foreach (var device in allDevices)
			{
				XManager.RemoveDevice(device);
			}
			var parent = Parent;
			if (parent != null)
			{
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var device in allDevices)
				{
					DevicesViewModel.Current.AllDevices.RemoveAll(x => x.Device.UID == device.UID);
				}
				DevicesViewModel.Current.AllDevices.Remove(this);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			Plans.Designer.Helper.BuildMap();
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.IsGroupDevice);
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			var devicesOnShleifViewModel = new DevicesOnShleifViewModel(Device);
			DialogService.ShowModalWindow(devicesOnShleifViewModel);
		}
		bool CanSelect()
		{
			return Driver.DriverType == XDriverType.KAU_Shleif || Driver.DriverType == XDriverType.RSR2_KAU_Shleif;
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
				return XManager.GetPresentationZone(Device);
			}
		}

		public string EditingPresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				var presentationZone = XManager.GetPresentationZone(Device);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (Driver.HasZone)
						presentationZone = "Нажмите для выбора зон";
					if (Driver.HasLogic)
						presentationZone = "Нажмите для настройки логики";
				}
				return presentationZone;
			}
		}

		bool _isZoneGrayed;
		public bool IsZoneGrayed
		{
			get { return _isZoneGrayed; }
			set
			{
				_isZoneGrayed = value;
				OnPropertyChanged("IsZoneGrayed");
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
			var plansElement = new ElementXDevice
				{
					XDeviceUID = Device.UID
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
			var brush = PictureCacheSource.XDevicePicture.GetXBrush(Device);
			return new Rectangle
			{
				Fill = brush,
				Height = PainterCache.PointZoom * PainterCache.Zoom,
				Width = PainterCache.PointZoom * PainterCache.Zoom,
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
			var deviceLogicViewModel = new DeviceLogicViewModel(Device, Device.DeviceLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				XManager.ChangeDeviceLogic(Device, deviceLogicViewModel.GetModel());
				OnPropertyChanged("PresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowLogic()
		{
			return Driver.HasLogic && !Device.IsNotUsed && !Device.IsChildMPTOrMRO();
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones, true);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				XManager.ChangeDeviceZones(Device, zonesSelectationViewModel.Zones);
				OnPropertyChanged("PresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowZones()
		{
			return Driver.HasZone && !Device.IsNotUsed;
		}

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			IsSelected = true;
			if (CanShowZones())
				OnShowZones();

			if (CanShowLogic())
				OnShowLogic();
		}
		bool CanShowZoneOrLogic()
		{
			return CanShowZones() || CanShowLogic();
		}

		public bool IsZoneOrLogic
		{
			get { return CanShowZoneOrLogic(); }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zone = Device.Zones.FirstOrDefault();
			if (zone != null)
			{
				ServiceFactoryBase.Events.GetEvent<ShowXZoneEvent>().Publish(zone.UID);
			}
		}
		bool CanShowZone()
		{
			return Device.Zones.Count == 1;
		}

		public RelayCommand ShowNSLogicCommand { get; private set; }
		void OnShowNSLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(Device, Device.NSLogic);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				Device.NSLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("NSPresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowNSLogic()
		{
			return (Driver.IsPump && Device.IntAddress <= 8) || Driver.DriverType == XDriverType.RSR2_Bush;
		}

		public bool IsNSLogic
		{
			get { return CanShowNSLogic(); }
		}

		public string NSPresentationZone
		{
			get
			{
				var presentationZone = XManager.GetPresentationZone(Device.NSLogic);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
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
		public XDriver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.DriverType != value.DriverType)
				{
					XManager.ChangeDriver(Device, value);
					Nodes.Clear();
					foreach (var childDevice in Device.Children)
					{
						DevicesViewModel.Current.AddDevice(childDevice, this);
					}
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					OnPropertyChanged("Device");
					OnPropertyChanged("Children");
					OnPropertyChanged("EditingPresentationZone");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
					if (Device.KAURSR2Parent != null)
						XManager.RebuildRSR2Addresses(Device.KAURSR2Parent);
					XManager.DeviceConfiguration.Update();
					Update();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		public ObservableCollection<XDriver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				switch (Device.Parent.DriverType)
				{
					case XDriverType.AM_4:
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AM_1));
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AM1_T));
						break;

					case XDriverType.AMP_4:
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AMP_1));
						break;

					default:
						foreach (var driverType in Device.Parent.Driver.Children)
						{
							var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == driverType);
							if (CanDriverBeChanged(driver))
							{
								AvailvableDrivers.Add(driver);
							}
						}
						break;
				}
			}
		}

		public bool CanDriverBeChanged(XDriver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (Device.IsChildMPTOrMRO())
				return false;

			if (Device.Parent.Driver.DriverType == XDriverType.AM_4)
				return true;

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
			ServiceFactoryBase.Events.GetEvent<ShowXDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public bool IsBold { get; set; }

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