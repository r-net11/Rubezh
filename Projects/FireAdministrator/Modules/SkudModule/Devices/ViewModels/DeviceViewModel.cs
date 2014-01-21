using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
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
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public SKDDevice Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(SKDDevice device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZonesCommand = new RelayCommand(OnShowZones, CanShowZones);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);

			device.Changed += OnChanged;
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
				Device.Address = value;
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
			get { return Device.Address; }
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
			var newDeviceViewModel = new NewDeviceViewModel(this);

			if (newDeviceViewModel.Drivers.Count == 1)
			{
				newDeviceViewModel.SaveCommand.Execute();
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevice;
				//Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.SKDChanged = true;
				return;
			}
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				//Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public bool CanAdd()
		{
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
			return ((Parent != null) && (Parent.AddCommand.CanExecute(null)));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var allDevices = Device.Children;
			foreach (var device in allDevices)
			{
				SKDManager.Devices.Remove(device);
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
			//Infrustructure.Plans.Designer.Helper.BuildMap();
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.IsGroupDevice);
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
				return "Zone";
				//return XManager.GetPresentationZone(Device);
			}
		}

		public string EditingPresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				//var presentationZone = XManager.GetPresentationZone(Device);
				var presentationZone = "Zone";
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
			//var brush = DevicePictureCache.GetXBrush(Device);
			return new Rectangle
			{
				//Fill = brush,
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

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			//var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones, true);
			//if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			//{
			//    XManager.ChangeDeviceZones(Device, zonesSelectationViewModel.Zones);
			//    OnPropertyChanged("PresentationZone");
			//    ServiceFactory.SaveService.GKChanged = true;
			//}
		}
		bool CanShowZones()
		{
			return Driver.HasZone && !Device.IsNotUsed;
		}

		public bool IsZoneOrLogic
		{
			get { return CanShowZones(); }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			//var zone = Device.Zones.FirstOrDefault();
			//if (zone != null)
			//{
			//    ServiceFactoryBase.Events.GetEvent<ShowXZoneEvent>().Publish(zone.UID);
			//}
		}
		bool CanShowZone()
		{
			return true;
			//return Device.Zones.Count == 1;
		}

		#endregion

		public SKDDriver Driver
		{
			get { return Device.Driver; }
		}

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
	}
}