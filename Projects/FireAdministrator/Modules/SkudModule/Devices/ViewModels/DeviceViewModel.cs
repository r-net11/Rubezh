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
using DeviceControls;

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
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowOuterZoneCommand = new RelayCommand(OnShowOuterZone, CanShowOuterZone);
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
			OnPropertyChanged("PresentationOuterZone");
			OnPropertyChanged("EditingPresentationZone");
			OnPropertyChanged("EditingPresentationOuterZone");
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
				OnPropertyChanged("Address");
				OnPropertyChanged("PresentationAddress");
				ServiceFactory.SaveService.SKDChanged = true;
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
				ServiceFactory.SaveService.SKDChanged = true;
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
				Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.SKDChanged = true;
				return;
			}
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				Plans.Designer.Helper.BuildMap();
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
			Plans.Designer.Helper.BuildMap();
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var readerDetailsViewModel = new ReaderDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(readerDetailsViewModel))
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanShowProperties()
		{
			return Device.DriverType == SKDDriverType.Reader;
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return Device.Driver.IsPlaceable; }
		}
		public VisualizationState VisualizationState
		{
			get { return Driver != null && Driver.IsPlaceable ? (IsOnPlan ? (Device.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent) : VisualizationState.Prohibit; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			IsSelected = true;
			var plansElement = new ElementSKDDevice
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
			var brush = DevicePictureCache.GetSKDBrush(Device);
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

		#region Zone
		public string PresentationZone
		{
			get { return SKDManager.GetPresentationZone(Device); }
		}

		public string EditingPresentationZone
		{
			get
			{
				var presentationZone = SKDManager.GetPresentationZone(Device);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (Driver.HasZone)
						presentationZone = "Нажмите для выбора зон";
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

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zoneSelectationViewModel = new ZoneSelectationViewModel(Device.ZoneUID, false);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				if (zoneSelectationViewModel.SelectedZone != null)
				{
					SKDManager.AddDeviceToZone(Device, zoneSelectationViewModel.SelectedZone.Zone);
				}
				OnPropertyChanged("PresentationZone");
				OnPropertyChanged("EditingPresentationZone");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanShowZone()
		{
			return Device.Driver.HasZone;
		}
		#endregion

		#region OuterZone
		public string PresentationOuterZone
		{
			get { return SKDManager.GetPresentationOuterZone(Device); }
		}

		public string EditingPresentationOuterZone
		{
			get
			{
				var presentationOuterZone = SKDManager.GetPresentationOuterZone(Device);
				IsOuterZoneGrayed = string.IsNullOrEmpty(presentationOuterZone);
				if (string.IsNullOrEmpty(presentationOuterZone))
				{
					if (Driver.HasOuterZone)
						presentationOuterZone = "Нажмите для выбора зон";
				}
				return presentationOuterZone;
			}
		}

		bool _isOuterZoneGrayed;
		public bool IsOuterZoneGrayed
		{
			get { return _isOuterZoneGrayed; }
			set
			{
				_isOuterZoneGrayed = value;
				OnPropertyChanged("IsOuterZoneGrayed");
			}
		}

		public RelayCommand ShowOuterZoneCommand { get; private set; }
		void OnShowOuterZone()
		{
			var zoneSelectationViewModel = new ZoneSelectationViewModel(Device.OuterZoneUID, true);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				if (zoneSelectationViewModel.SelectedZone != null)
				{
					SKDManager.AddDeviceToOuterZone(Device, zoneSelectationViewModel.SelectedZone.Zone);
				}
				OnPropertyChanged("PresentationOuterZone");
				OnPropertyChanged("EditingPresentationOuterZone");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanShowOuterZone()
		{
			return Device.Driver.HasOuterZone;
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
	}
}