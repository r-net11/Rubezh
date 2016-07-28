using System;
using System.Windows;
using System.Windows.Shapes;
using Common;
using DeviceControls;
using Localization.Strazh.ViewModels;
using Infrastructure.Events;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using StrazhModule.Events;
using StrazhModule.Plans;
using FiresecClient;

namespace StrazhModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public SKDDevice Device { get; private set; }

		private PropertiesViewModel _propertiesViewModel;
		public PropertiesViewModel PropertiesViewModel
		{
			get { return _propertiesViewModel; }
			set
			{
				_propertiesViewModel = value;
				OnPropertyChanged(() => PropertiesViewModel);
			}
		}

		public DeviceViewModel(SKDDevice device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			SearchDevicesCommand = new RelayCommand(OnSearchDevices);
			ChangeZoneCommand = new RelayCommand(OnChangeZone, CanChangeZone);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowDoorCommand = new RelayCommand(OnShowDoor, CanShowDoor);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);
			device.Changed += OnChanged;

			Update();
		}

		void OnChanged()
		{
			OnPropertyChanged(() => Address);
			OnPropertyChanged(() => PresentationZone);
			OnPropertyChanged(() => EditingPresentationZone);
			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => HasDoor);
		}

		public void UpdateProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged(() => PropertiesViewModel);
			OnChanged();
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
			IsEnabled = Device.IsEnabled;
			OnPropertyChanged(() => PresentationZone);
			OnPropertyChanged(() => EditingPresentationZone);
		}

		public string Address
		{
			get { return Device.Address; }
		}

		public string Name
		{
			get { return Device.Name; }
			set
			{
				Device.Name = value;
				OnPropertyChanged(() => Name);
				SKDManager.EditDevice(Device);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public SKDDoor Door
		{
			get { return Device.Door; }
		}

		public bool HasDoor
		{
			get { return Device.Door != null; }
		}

		public SKDDriver Driver
		{
			get { return Device.Driver; }
		}

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var rootDeviceViewModel = DevicesViewModel.Current.RootDevice;

			var newDeviceViewModel = new NewDeviceViewModel(rootDeviceViewModel);
			var result = false;
			if (newDeviceViewModel.Drivers.Count == 1)
			{
				newDeviceViewModel.SaveCommand.Execute();
				result = true;
			}
			else
			{
				result = DialogService.ShowModalWindow(newDeviceViewModel);
			}

			if (result)
			{
				var deviceViewModel = new DeviceViewModel(newDeviceViewModel.AddedDevice);
				rootDeviceViewModel.AddChild(deviceViewModel);
				DevicesViewModel.Current.AllDevices.Add(deviceViewModel);
				DevicesViewModel.Current.SelectedDevice = deviceViewModel;

				foreach (var childDevice in newDeviceViewModel.AddedDevice.Children)
				{
					var childDeviceViewModel = new DeviceViewModel(childDevice);
					deviceViewModel.AddChild(childDeviceViewModel);
					DevicesViewModel.Current.AllDevices.Add(childDeviceViewModel);
				}

				rootDeviceViewModel.Update();
				SKDPlanExtension.Instance.Cache.BuildSafe<SKDDevice>();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public bool CanAdd()
		{
			return true;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (!MessageBoxService.ShowConfirmation(String.Format(CommonViewModels.Controller_DeleteConfirm, Device.Name)))
				return;

			Logger.Info(String.Format("Удаление контроллера GUID='{0}'", Device.UID));
			var allDevices = Device.Children;
			SKDManager.DeleteDevice(Device);
			var parent = Parent;
			if (parent != null)
			{
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var device in allDevices)
					DevicesViewModel.Current.AllDevices.RemoveAll(x => x.Device.UID == device.UID);
				DevicesViewModel.Current.AllDevices.Remove(this);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			ServiceFactory.SaveService.SKDChanged = true;

			// Уведомляем всех заинтересованных об удалении контроллера
			ServiceFactoryBase.Events.GetEvent<ControllerDeletedEvent>().Publish(Device.UID);
		}
		bool CanRemove()
		{
			return Driver.IsController;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Driver.IsController)
			{
				var deviceInfoResult = FiresecManager.FiresecService.SKDGetDeviceInfo(Device);
				if (!deviceInfoResult.HasError)
				{
					var controllerPropertiesViewModel = new ControllerPropertiesViewModel(Device, deviceInfoResult.Result);
					DialogService.ShowModalWindow(controllerPropertiesViewModel);
				}
				else
				{
					MessageBoxService.ShowWarning(deviceInfoResult.Error);
				}
			}

			if (Driver.DriverType == SKDDriverType.Lock)
			{
				var lockPropertiesViewModel = new LockPropertiesViewModel(Device);
				if (DialogService.ShowModalWindow(lockPropertiesViewModel))
				{
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}

			if (Driver.DriverType == SKDDriverType.Reader)
			{
				var readerPropertiesViewModel = new ReaderPropertiesViewModel(Device);
				if (DialogService.ShowModalWindow(readerPropertiesViewModel))
				{
					Device.OnChanged();
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
		}
		bool CanShowProperties()
		{
			return Driver.IsController || (Driver.DriverType == SKDDriverType.Lock && Device.IsEnabled);
		}

		public RelayCommand SearchDevicesCommand { get; private set; }
		private void OnSearchDevices()
		{
			var rootDeviceViewModel = DevicesViewModel.Current.RootDevice;
			var searchDevicesViewModel = new SearchDevicesViewModel(rootDeviceViewModel);
			var result = DialogService.ShowModalWindow(searchDevicesViewModel);
			if (!result) return;
			foreach (var device in searchDevicesViewModel.AddedDevices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				rootDeviceViewModel.AddChild(deviceViewModel);
				DevicesViewModel.Current.AllDevices.Add(deviceViewModel);
				DevicesViewModel.Current.SelectedDevice = deviceViewModel;

				foreach (var childDevice in device.Children)
				{
					var childDeviceViewModel = new DeviceViewModel(childDevice);
					deviceViewModel.AddChild(childDeviceViewModel);
					DevicesViewModel.Current.AllDevices.Add(childDeviceViewModel);
				}
			}
			rootDeviceViewModel.Update();
			SKDPlanExtension.Instance.Cache.BuildSafe<SKDDevice>();
			ServiceFactory.SaveService.SKDChanged = true;
		}

		#region Plan
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
			ServiceFactory.Layout.SetRightPanelVisible(true);
			var brush = PictureCacheSource.SKDDevicePicture.GetBrush(Device);
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
		#endregion

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
						presentationZone = CommonViewModels.Zone_Select;
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
				OnPropertyChanged(() => IsZoneGrayed);
			}
		}

		public RelayCommand ChangeZoneCommand { get; private set; }
		void OnChangeZone()
		{
			IsSelected = true;
			var zoneSelectationViewModel = new ZoneSelectationViewModel(Device.ZoneUID);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				if (zoneSelectationViewModel.SelectedZone != null)
				{
					SKDManager.ChangeDeviceZone(Device, zoneSelectationViewModel.SelectedZone.Zone);
				}
				OnPropertyChanged(() => PresentationZone);
				OnPropertyChanged(() => EditingPresentationZone);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanChangeZone()
		{
			return Device.Driver.HasZone;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Device.ZoneUID);
		}
		bool CanShowZone()
		{
			return Device.Driver.HasZone && Device.ZoneUID != Guid.Empty;
		}
		#endregion

		public RelayCommand ShowDoorCommand { get; private set; }
		void OnShowDoor()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDoorEvent>().Publish(Device.Door.UID);
		}
		bool CanShowDoor()
		{
			return Device.Door != null;
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactoryBase.Events.GetEvent<ShowSKDDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public bool IsBold { get; set; }
	}
}