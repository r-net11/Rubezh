using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using DeviceControls;
using DevicesModule.DeviceProperties;
using DevicesModule.Plans.Designer;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Plans.Painters;

namespace DevicesModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(Device device, bool intitialize = true)
		{
			Device = device;
			if (!intitialize)
				return;

			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			CreateDragVisual = OnCreateDragVisual;
			PropertiesViewModel = new PropertiesViewModel(device);
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			AvailvableDrivers = new ObservableCollection<Driver>();
			UpdateDriver();
			device.Changed += new Action(device_Changed);

			UpdateZoneName();
		}

		void device_Changed()
		{
			OnPropertyChanged(() => Address);
			OnPropertyChanged(() => HasExternalDevices);
			UpdateZoneName();
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public string Address
		{
			get { return Device.PresentationAddress; }
			set
			{
				if (Device.SetAddress(value))
				{
					if (Driver.IsChildAddressReservedRange)
					{
						foreach (var deviceViewModel in Children)
						{
							deviceViewModel.OnPropertyChanged(() => Address);
						}
					}
					ServiceFactory.SaveService.FSChanged = true;
				}
				OnPropertyChanged(() => Address);
			}
		}

		public bool HasDifferences
		{
			get { return Device.HasDifferences; }
			set { }
		}
		public bool HasMissingDifferences
		{
			get { return Device.HasMissingDifferences; }
			set { }
		}

		public string XXXPresentationZone { get; set; }

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				Device.OnChanged();
				OnPropertyChanged(Description);
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		#region Zone
		public bool IsZoneDevice
		{
			get { return Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device); }
		}

		void UpdateZoneName()
		{
			if (Device.IsNotUsed)
				PresentationZone = null;
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);

			if (Device.IsNotUsed)
				EditingPresentationZone = null;
			var presentationZone = PresentationZone;
			IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
			if (string.IsNullOrEmpty(presentationZone))
			{
				if (Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device))
					presentationZone = "Нажмите для выбора зон";
				if (Driver.IsZoneLogicDevice && !FiresecManager.FiresecConfiguration.IsChildMRO2(Device))
					presentationZone = "Нажмите для настройки логики";
				if (Driver.DriverType == DriverType.Indicator)
					presentationZone = "Нажмите для настройки индикатора";
				if (Driver.DriverType == DriverType.PDUDirection)
					presentationZone = "Нажмите для выбора устройств";
			}
			EditingPresentationZone = presentationZone;
		}

		string _presentationZone;
		public string PresentationZone
		{
			get { return _presentationZone; }
			set
			{
				_presentationZone = value;
				OnPropertyChanged(() => PresentationZone);
			}
		}

		string _editingPresentationZone;
		public string EditingPresentationZone
		{
			get { return _editingPresentationZone; }
			set
			{
				_editingPresentationZone = value;
				OnPropertyChanged(() => EditingPresentationZone);
			}
		}

		public bool IsZoneOrLogic
		{
			get { return Driver.IsZoneDevice || Driver.IsZoneLogicDevice || Driver.DriverType == DriverType.Indicator || Driver.DriverType == DriverType.PDUDirection; }
		}

		bool _isZoneGrayed = false;
		public bool IsZoneGrayed
		{
			get { return _isZoneGrayed; }
			set
			{
				_isZoneGrayed = value;
				OnPropertyChanged(() => IsZoneGrayed);
			}
		}

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			if (Driver.IsZoneDevice)
			{
				if (!FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				{
					var zoneSelectationViewModel = new ZoneSelectationViewModel(Device);
					if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					{
						ServiceFactory.SaveService.FSChanged = true;
					}
				}
			}
			if (Driver.IsZoneLogicDevice)
			{
				if (CanShowZoneLogic())
					OnShowZoneLogic();
			}
			if (Driver.DriverType == DriverType.Indicator)
			{
				OnShowIndicatorLogic();
			}
			if (Driver.DriverType == DriverType.PDUDirection)
			{
				if (DialogService.ShowModalWindow(new PDUDetailsViewModel(Device)))
					ServiceFactory.SaveService.FSChanged = true;
			}
			UpdateZoneName();
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				ServiceFactory.SaveService.FSChanged = true;
			}
			UpdateZoneName();
		}
		bool CanShowZoneLogic()
		{
			return (Driver.IsZoneLogicDevice && !FiresecManager.FiresecConfiguration.IsChildMRO2(Device) && !Device.IsNotUsed);
		}
		#endregion

		public bool HasExternalDevices
		{
			get { return Device.HasExternalDevices; }
		}

		public string ConnectedTo
		{
			get { return Device.ConnectedTo; }
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				FiresecManager.FiresecConfiguration.SetIsNotUsed(Device, !value);
				OnPropertyChanged(() => IsUsed);
				OnPropertyChanged(() => ShowOnPlan);
				UpdateZoneName();
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsPlaceable || Device.Children.Count > 0); }
		}

		public VisualizationState VisualizationState
		{
			get { return Driver != null && Driver.IsPlaceable ? (IsOnPlan ? (Device.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent) : VisualizationState.Prohibit; }
		}

		void OnShowIndicatorLogic()
		{
			if (Device.IndicatorLogic.Device != null && Device.IndicatorLogic.Device.Driver.DriverType == DriverType.Indicator)
			{
				MessageBoxService.ShowError("Разрешено редактировать только исходный индикатор, привязанный к НС");
				return;
			}
			var indicatorDetailsViewModel = new IndicatorDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(indicatorDetailsViewModel))
				ServiceFactory.SaveService.FSChanged = true;
			UpdateZoneName();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var newDeviceViewModel = new NewDeviceViewModel(this);
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				ServiceFactory.SaveService.FSChanged = true;
				DevicesViewModel.UpdateGuardVisibility();
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.CreatedDeviceViewModel);
				Helper.BuildMap();
			}
		}
		public bool CanAdd()
		{
			if (FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				return false;
			if (FiresecManager.FiresecConfiguration.IsChildMRO2(Device))
				return false;
			return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
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
			if (Driver.IsPanel)
			{
				if (!MessageBoxService.ShowQuestion("Вы действительно хотите удалить устройство"))
					return;
			}

			FiresecManager.FiresecConfiguration.RemoveDevice(Device);
			var parent = Parent;
			if (parent != null)
			{
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				ServiceFactory.SaveService.FSChanged = true;
				DevicesViewModel.UpdateGuardVisibility();

				index = Math.Min(index, parent.ChildrenCount - 1);
				DevicesViewModel.Current.AllDevices.Remove(this);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			Helper.BuildMap();
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.AutoChild == Driver.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.Page:
					if (DialogService.ShowModalWindow(new IndicatorPageDetailsViewModel(Device)))
					{
						ServiceFactory.SaveService.FSChanged = true;
						foreach (DeviceViewModel deviceViewModel in Children)
						{
							deviceViewModel.UpdateZoneName();
						}
					}
					break;

				case DriverType.Indicator:
					OnShowIndicatorLogic();
					break;

				case DriverType.Valve:
					if (DialogService.ShowModalWindow(new ValveDetailsViewModel(Device)))
					{
						ServiceFactory.SaveService.FSChanged = true;
						OnPropertyChanged(() => HasExternalDevices);
					}
					break;

				case DriverType.Pump:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.CompensationPump:
					if (DialogService.ShowModalWindow(new PumpDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.PDUDirection:
					if (DialogService.ShowModalWindow(new PDUDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.UOO_TL:
					if (DialogService.ShowModalWindow(new UOOTLDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.MS_3:
				case DriverType.MS_4:
					if (DialogService.ShowModalWindow(new MS34DetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.MPT:
					if (DialogService.ShowModalWindow(new MptDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;
			}
			UpdateZoneName();
		}
		bool CanShowProperties()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.Page:
				case DriverType.Indicator:
				case DriverType.Valve:
				case DriverType.Pump:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.CompensationPump:
				case DriverType.PDUDirection:
				case DriverType.UOO_TL:
				case DriverType.MS_3:
				case DriverType.MS_4:
				case DriverType.MPT:
					return true;
			}
			return false;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			if (Device.ZoneUID != Guid.Empty)
				ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneUID);
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

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			IsSelected = true;
			var plansElement = new ElementDevice()
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
			var brush = PictureCacheSource.DevicePicture.GetBrush(Device);
			return new Rectangle()
			{
				Fill = brush,
				Height = PainterCache.DefaultPointSize,
				Width = PainterCache.DefaultPointSize,
			};
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.Driver.DriverType != value.DriverType)
				{
					if (value.DriverType == DriverType.MS_1)
					{
						if (Device.Children.Count == 2)
						{
							FiresecManager.FiresecConfiguration.RemoveDevice(Device.Children[1]);
							if (Device.Children.Count > 1)
								Device.Children.RemoveAt(1);
						}
					}
					if (value.DriverType == DriverType.MS_2)
					{
						if (Device.Children.Count == 1)
						{
							FiresecManager.FiresecConfiguration.RemoveDevice(Device.Children[0]);
							var channelDevice = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.USB_Channel_2);
							AddDeviceOnDriverChanged(channelDevice, 0);
						}
					}

					if (value.IsPanel)
					{
						var devicesToRemove = new List<DeviceViewModel>();
						if (value.ShleifCount < Device.Driver.ShleifCount)
						{
							foreach (DeviceViewModel child in Children)
							{
								if (child.Device.IntAddress / 256 > value.ShleifCount)
									devicesToRemove.Add(child);
							}
						}
						foreach (DeviceViewModel child in Children)
						{
							if (Driver.AutoCreateChildren.Contains(child.Driver.UID) && !value.AutoCreateChildren.Contains(child.Driver.UID))
								devicesToRemove.Add(child);
						}
						foreach (var deviceToRemove in devicesToRemove)
						{
							FiresecManager.FiresecConfiguration.RemoveDevice(deviceToRemove.Device);
							Nodes.Remove(deviceToRemove);
						}

						foreach (var autoCreateDriverId in value.AutoCreateChildren)
						{
							var autoCreateDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);

							for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
							{
								if (!Device.Children.Any(x => x.IntAddress == i))
									AddDeviceOnDriverChanged(autoCreateDriver, i);
							}
						}

						Device.Driver = value;
						Device.DriverUID = value.UID;
					}
					else
					{
						FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
						for (int i = Device.Children.Count - 1; i > 0; i--)
						{
							var child = Device.Children[i];
							FiresecManager.FiresecConfiguration.RemoveDevice(child);
						}
						for (int i = ChildrenCount - 1; i > 0; i--)
						{
							var child = this[i];
							DevicesViewModel.Current.AllDevices.Remove(child);
						}

						Nodes.Clear();
						Device.Children.Clear();

						foreach (var autoCreateDriverId in value.AutoCreateChildren)
						{
							var autoCreateDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);
							for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
							{
								if (!Device.Children.Any(x => x.IntAddress == i))
									AddDeviceOnDriverChanged(autoCreateDriver, i);
							}
						}
					}

					Device.Driver = value;
					Device.DriverUID = value.UID;

					OnPropertyChanged(() => Device);
					OnPropertyChanged(() => Driver);
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged(() => PropertiesViewModel);
					Update();
					ServiceFactory.SaveService.FSChanged = true;
					DevicesViewModel.UpdateGuardVisibility();
				}
			}
		}

		void AddDeviceOnDriverChanged(Driver driver, int address)
		{
			var device = new Device()
			{
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = address,
				Parent = Device
			};
			Device.Children.Insert(0, device);
			var deviceViewModel = new DeviceViewModel(device);
			Nodes.Insert(0, deviceViewModel);
			DevicesViewModel.Current.AllDevices.Add(deviceViewModel);
			FiresecManager.Devices.Add(device);
		}

		public ObservableCollection<Driver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
#if DEBUG
				if (Driver.DriverType == DriverType.MS_1 || Driver.DriverType == DriverType.MS_2)
				{
					AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_1));
					AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_2));
					return;
				}
#endif

				switch (Device.Parent.Driver.DriverType)
				{
					case DriverType.AM4:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM_1));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StopButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StartButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AutomaticButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOnButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOffButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzUnblockButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_T));
						if (Device.Parent.Parent.Driver.DriverType == DriverType.Rubezh_2OP || Device.Parent.Parent.Driver.DriverType == DriverType.USB_Rubezh_2OP)
							AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						return;

					case DriverType.AM4_P:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4));
						if (Device.Parent.Parent.Driver.DriverType == DriverType.Rubezh_2OP || Device.Parent.Parent.Driver.DriverType == DriverType.USB_Rubezh_2OP)
							AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						return;

					default:
						foreach (var driverUID in Device.Parent.Driver.AvaliableChildren)
						{
							var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
							if (CanDriverBeChanged(driver))
							{
								AvailvableDrivers.Add(driver);
							}
						}
						if (Device.Parent.Driver.DriverType != DriverType.Rubezh_2OP && Device.Parent.Driver.DriverType != DriverType.USB_Rubezh_2OP)
						{
							var am1_o_Driver = AvailvableDrivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O);
							if (am1_o_Driver != null)
							{
								AvailvableDrivers.Remove(am1_o_Driver);
							}
						}
						break;
				}
			}
		}

		public bool CanDriverBeChanged(Driver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (Device.Parent.Driver.DriverType == DriverType.AM4)
				return true;
			if (Device.Parent.Driver.DriverType == DriverType.AM4_P)
				return true;

			if (driver.IsAutoCreate)
				return false;

			if (Device.Parent.Driver.AutoChildCount > 0)
				return false;

			if (driver.IsChildAddressReservedRange)
				return true;

			if (driver.IsPanel)
				return true;

#if DEBUG
			if (Driver.DriverType == DriverType.MS_1 || Driver.DriverType == DriverType.MS_2)
				return true;
#endif

			return (driver.Category == DeviceCategoryType.Sensor) || (driver.Category == DeviceCategoryType.Effector);
		}

		public bool CanChangeDriver
		{
			get { return CanDriverBeChanged(Device.Driver); }
		}

		public bool IsBold { get; set; }

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }

		public override string ToString()
		{
			return Device.FullPresentationName;
		}
	}
}