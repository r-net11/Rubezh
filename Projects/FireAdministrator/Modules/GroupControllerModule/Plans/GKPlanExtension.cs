using Common;
using DeviceControls;
using GKModule.Events;
using GKModule.Plans.Designer;
using GKModule.Plans.InstrumentAdorners;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Interfaces;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Services;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
//using System.Windows.Media;

namespace GKModule.Plans
{
	class GKPlanExtension : BasePlanExtension
	{
		public static GKPlanExtension Instance { get; private set; }

		private bool _processChanges;
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private GuardZonesViewModel _guardZonesViewModel;
		private SKDZonesViewModel _skdZonesViewModel;
		private DirectionsViewModel _directionsViewModel;
		private MPTsViewModel _mptsViewModel;
		private DoorsViewModel _doorsViewModel;
		private DelaysViewModel _delaysViewModel;
		private PumpStationsViewModel _pumpStationsViewModel;
		private IEnumerable<IInstrument> _instruments;
		private List<DesignerItem> _designerItems;

		public GKPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, GuardZonesViewModel guardZonesViewModel, SKDZonesViewModel skdZonesViewModel, DelaysViewModel delaysViewModel, PumpStationsViewModel pumpStationsViewModel, DirectionsViewModel directionsViewModel, MPTsViewModel mptsViewModel, DoorsViewModel doorsViewModel)
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(UpdateGKDeviceInGKZones);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(UpdateGKDeviceInGKZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(UpdateGKDeviceInGKZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(UpdateGKDeviceInGKZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateGKDeviceInGKZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateGKDeviceInGKZones);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_guardZonesViewModel = guardZonesViewModel;
			_skdZonesViewModel = skdZonesViewModel;
			_directionsViewModel = directionsViewModel;
			_mptsViewModel = mptsViewModel;
			_doorsViewModel = doorsViewModel;
			_delaysViewModel = delaysViewModel;
			this._pumpStationsViewModel = pumpStationsViewModel;

			_instruments = null;
			_processChanges = true;
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKZone>(() => GKManager.Zones);
			Cache.Add<GKGuardZone>(() => GKManager.DeviceConfiguration.GuardZones);
			Cache.Add<GKSKDZone>(() => GKManager.SKDZones);
			Cache.Add<GKDelay>(() => GKManager.Delays);
			Cache.Add<GKDirection>(() => GKManager.Directions);
			Cache.Add<GKMPT>(() => GKManager.MPTs);
			Cache.Add<GKDoor>(() => GKManager.Doors);
			_designerItems = new List<DesignerItem>();
		}

		public override void Initialize()
		{
			base.Initialize();
			PictureCacheSource.GKDevicePicture.LoadCache();
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return "Устройства"; }
		}

		public override IEnumerable<IInstrument> Instruments
		{
			get
			{
				if (_instruments == null)
					_instruments = new List<IInstrument>()
					{
						new InstrumentViewModel()
						{
							ImageSource="ZoneRectangle",
							ToolTip="Пожарная зона",
							Adorner = new ZoneRectangleAdorner(DesignerCanvas, _zonesViewModel),
							Index = 200,
							Autostart = true,
							GroupIndex = 200,
						},
						new InstrumentViewModel()
						{
							ImageSource="ZonePolygon",
							ToolTip="Пожарная зона",
							Adorner = new ZonePolygonAdorner(DesignerCanvas, _zonesViewModel),
							Index = 201,
							Autostart = true,
							GroupIndex = 200,
						},
						new InstrumentViewModel()
						{
							ImageSource="ZoneRectangle",
							ToolTip="Охранная зона",
							Adorner = new GuardZoneRectangleAdorner(DesignerCanvas, _guardZonesViewModel),
							Index = 204,
							Autostart = true,
							GroupIndex = 204,
						},
						new InstrumentViewModel()
						{
							ImageSource="ZonePolygon",
							ToolTip="Охранная зона",
							Adorner = new GuardZonePolygonAdorner(DesignerCanvas,  _guardZonesViewModel),
							Index = 205,
							Autostart = true,
							GroupIndex = 204,
						},
						new InstrumentViewModel()
						{
							ImageSource="ZoneRectangle",
							ToolTip="СКД Зона",
							Adorner = new SKDZoneRectangleAdorner(DesignerCanvas, _skdZonesViewModel),
							Index = 206,
							Autostart = true,
							GroupIndex = 206,
						},
						new InstrumentViewModel()
						{
							ImageSource="ZonePolygon",
							ToolTip="СКД Зона",
							Adorner = new SKDZonePolygonAdorner(DesignerCanvas, _skdZonesViewModel),
							Index = 207,
							Autostart = true,
							GroupIndex = 206,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionRectangle",
							ToolTip="Направление",
							Adorner = new DirectionRectangleAdorner(DesignerCanvas, _directionsViewModel),
							Index = 209,
							Autostart = true,
							GroupIndex = 208,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionPolygon",
							ToolTip="Направление",
							Adorner = new DirectionPolygonAdorner(DesignerCanvas, _directionsViewModel),
							Index = 210,
							Autostart = true,
							GroupIndex = 208,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionRectangle",
							ToolTip="МПТ",
							Adorner = new MPTRectangleAdorner(DesignerCanvas, _mptsViewModel),
							Index = 211,
							Autostart = true,
							GroupIndex = 210,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionPolygon",
							ToolTip="МПТ",
							Adorner = new MPTPolygonAdorner(DesignerCanvas, _mptsViewModel),
							Index = 212,
							Autostart = true,
							GroupIndex = 210,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionRectangle",
							ToolTip="Задержки",
							Adorner = new DelayRectangleAdorner(DesignerCanvas, _delaysViewModel),
							Index = 213,
							Autostart = true,
							GroupIndex = 212,
						},
						new InstrumentViewModel()
						{
							ImageSource="DirectionPolygon",
							ToolTip="Задержки",
							Adorner = new DelayPolygonAdorner(DesignerCanvas, _delaysViewModel),
							Index = 214,
							Autostart = true,
							GroupIndex = 212,
						},
						new InstrumentViewModel()
						{
							ImageSource = "DirectionRectangle",
							ToolTip = "Насосные станции",
							Adorner = new PumpStationRectangleAdorner(DesignerCanvas, this._pumpStationsViewModel),
							Index = 215,
							Autostart = true,
							GroupIndex = 214,
						},
						new InstrumentViewModel()
						{
							ImageSource = "DirectionPolygon",
							ToolTip = "Насосные станции",
							Adorner = new PumpStationPolygonAdorner(DesignerCanvas, this._pumpStationsViewModel),
							Index = 216,
							Autostart = true,
							GroupIndex = 214,
						}
				};
				return _instruments;
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementGKDevice)
			{
				var elementGKDevice = element as ElementGKDevice;
				plan.ElementGKDevices.Add(elementGKDevice);
				SetItem<GKDevice>(elementGKDevice);
				return true;
			}
			else if (element is ElementGKDoor)
			{
				var elementGKDoor = (ElementGKDoor)element;
				plan.ElementGKDoors.Add(elementGKDoor);
				SetItem<GKDoor>(elementGKDoor);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleGKZone)
				{
					var elementRectangleGKZone = (ElementRectangleGKZone)element;
					plan.ElementRectangleGKZones.Add(elementRectangleGKZone);
					SetItem<GKZone>(elementRectangleGKZone);
				}
				else if (element is ElementPolygonGKZone)
				{
					var elementPolygonGKZone = (ElementPolygonGKZone)element;
					plan.ElementPolygonGKZones.Add(elementPolygonGKZone);
					SetItem<GKZone>(elementPolygonGKZone);
				}
				else if (element is ElementRectangleGKGuardZone)
				{
					var elementRectangleGKGuardZone = (ElementRectangleGKGuardZone)element;
					plan.ElementRectangleGKGuardZones.Add(elementRectangleGKGuardZone);
					SetItem<GKGuardZone>(elementRectangleGKGuardZone);
				}
				else if (element is ElementPolygonGKGuardZone)
				{
					var elementPolygonGKGuardZone = (ElementPolygonGKGuardZone)element;
					plan.ElementPolygonGKGuardZones.Add(elementPolygonGKGuardZone);
					SetItem<GKGuardZone>(elementPolygonGKGuardZone);
				}
				else if (element is ElementRectangleGKSKDZone)
				{
					var elementRectangleGKSKDZone = (ElementRectangleGKSKDZone)element;
					plan.ElementRectangleGKSKDZones.Add(elementRectangleGKSKDZone);
					SetItem<GKSKDZone>(elementRectangleGKSKDZone);
				}
				else if (element is ElementPolygonGKSKDZone)
				{
					var elementPolygonGKSKDZone = (ElementPolygonGKSKDZone)element;
					plan.ElementPolygonGKSKDZones.Add(elementPolygonGKSKDZone);
					SetItem<GKSKDZone>(elementPolygonGKSKDZone);
				}
				else
					return false;
				return true;
			}
			else if (element is IElementDelay)
			{
				if (element is ElementRectangleGKDelay)
					plan.ElementRectangleGKDelays.Add((ElementRectangleGKDelay)element);
				else if (element is ElementPolygonGKDelay)
					plan.ElementPolygonGKDelays.Add((ElementPolygonGKDelay)element);
				else
					return false;
				SetItem<GKDelay>((IElementDelay)element);
				return true;
			}
			else if (element is IElementPumpStation)
			{
				if (element is ElementRectangleGKPumpStation)
					plan.ElementRectangleGKPumpStations.Add((ElementRectangleGKPumpStation)element);
				else if (element is ElementPolygonGKPumpStation)
					plan.ElementPolygonGKPumpStations.Add((ElementPolygonGKPumpStation)element);
				else
					return false;
				SetItem<GKPumpStation>((IElementPumpStation)element);
				return true;
			}
			else if (element is IElementDirection)
			{
				if (element is ElementRectangleGKDirection)
					plan.ElementRectangleGKDirections.Add((ElementRectangleGKDirection)element);
				else if (element is ElementPolygonGKDirection)
					plan.ElementPolygonGKDirections.Add((ElementPolygonGKDirection)element);
				else
					return false;
				SetItem<GKDirection>((IElementDirection)element);
				return true;
			}
			else if (element is IElementMPT)
			{
				if (element is ElementRectangleGKMPT)
					plan.ElementRectangleGKMPTs.Add((ElementRectangleGKMPT)element);
				else if (element is ElementPolygonGKMPT)
					plan.ElementPolygonGKMPTs.Add((ElementPolygonGKMPT)element);
				else
					return false;
				SetItem<GKMPT>((IElementMPT)element);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementGKDevice)
			{
				var elementDevice = (ElementGKDevice)element;
				plan.ElementGKDevices.Remove(elementDevice);
				ResetItem<GKDevice>(elementDevice);
				return true;
			}
			else if (element is ElementGKDoor)
			{
				var elementGKDoor = (ElementGKDoor)element;
				plan.ElementGKDoors.Remove(elementGKDoor);
				ResetItem<GKDoor>(elementGKDoor);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleGKZone)
					plan.ElementRectangleGKZones.Remove((ElementRectangleGKZone)element);
				else if (element is ElementPolygonGKZone)
					plan.ElementPolygonGKZones.Remove((ElementPolygonGKZone)element);
				else if (element is ElementRectangleGKGuardZone)
					plan.ElementRectangleGKGuardZones.Remove((ElementRectangleGKGuardZone)element);
				else if (element is ElementPolygonGKGuardZone)
					plan.ElementPolygonGKGuardZones.Remove((ElementPolygonGKGuardZone)element);
				else if (element is ElementRectangleGKSKDZone)
					plan.ElementRectangleGKSKDZones.Remove((ElementRectangleGKSKDZone)element);
				else if (element is ElementPolygonGKSKDZone)
					plan.ElementPolygonGKSKDZones.Remove((ElementPolygonGKSKDZone)element);
				else
					return false;
				ResetItem<GKZone>((IElementZone)element);
				ResetItem<GKGuardZone>((IElementZone)element);
				ResetItem<GKSKDZone>((IElementZone)element);
				ResetItem<GKDelay>((IElementZone)element);
				return true;
			}
			else if (element is IElementDelay)
			{
				if (element is ElementRectangleGKDelay)
					plan.ElementRectangleGKDelays.Remove((ElementRectangleGKDelay)element);
				else if (element is ElementPolygonGKDelay)
					plan.ElementPolygonGKDelays.Remove((ElementPolygonGKDelay)element);
				else
					return false;
				ResetItem<GKDelay>((IElementDelay)element);
				return true;
			}
			else if (element is IElementPumpStation)
			{
				if (element is ElementRectangleGKPumpStation)
					plan.ElementRectangleGKPumpStations.Remove((ElementRectangleGKPumpStation)element);
				else if (element is ElementPolygonGKPumpStation)
					plan.ElementPolygonGKPumpStations.Remove((ElementPolygonGKPumpStation)element);
				else
					return false;
				ResetItem<GKPumpStation>((IElementPumpStation)element);
				return true;
			}
			else if (element is IElementDirection)
			{
				if (element is ElementRectangleGKDirection)
					plan.ElementRectangleGKDirections.Remove((ElementRectangleGKDirection)element);
				else if (element is ElementPolygonGKDirection)
					plan.ElementPolygonGKDirections.Remove((ElementPolygonGKDirection)element);
				else
					return false;
				ResetItem<GKDirection>((IElementDirection)element);
				return true;
			}
			else if (element is IElementMPT)
			{
				if (element is ElementRectangleGKMPT)
					plan.ElementRectangleGKMPTs.Remove((ElementRectangleGKMPT)element);
				else if (element is ElementPolygonGKMPT)
					plan.ElementPolygonGKMPTs.Remove((ElementPolygonGKMPT)element);
				else
					return false;
				ResetItem<GKMPT>((IElementMPT)element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleGKZone || designerItem.Element is ElementPolygonGKZone)
				RegisterDesignerItem<GKZone>(designerItem, "GKZone", "/Controls;component/Images/Zone.png");
			else if (designerItem.Element is ElementRectangleGKGuardZone || designerItem.Element is ElementPolygonGKGuardZone)
				RegisterDesignerItem<GKGuardZone>(designerItem, "GKGuardZone", "/Controls;component/Images/GuardZone.png");
			else if (designerItem.Element is ElementRectangleGKDelay || designerItem.Element is ElementPolygonGKDelay)
				RegisterDesignerItem<GKDelay>(designerItem, "GKDelay", "/Controls;component/Images/Delay.png");
			else if (designerItem.Element is ElementRectangleGKPumpStation || designerItem.Element is ElementPolygonGKPumpStation)
				RegisterDesignerItem<GKPumpStation>(designerItem, "GKPumpStation", "/Controls;component/Images/PumpStation.png");
			else if (designerItem.Element is ElementRectangleGKSKDZone || designerItem.Element is ElementPolygonGKSKDZone)
				RegisterDesignerItem<GKSKDZone>(designerItem, "GKSKDZone", "/Controls;component/Images/SKDZone.png");
			else if (designerItem.Element is ElementGKDoor)
				RegisterDesignerItem<GKDoor>(designerItem, "GKDoors", "/Controls;component/Images/Door.png");
			else if (designerItem.Element is ElementGKDevice)
			{
				RegisterDesignerItem<GKDevice>(designerItem, "GK");
				_designerItems.Add(designerItem);
			}
			else if (designerItem.Element is IElementDirection)
				RegisterDesignerItem<GKDirection>(designerItem, "GKDirection", "/Controls;component/Images/Blue_Direction.png");
			else if (designerItem.Element is IElementMPT)
				RegisterDesignerItem<GKMPT>(designerItem, "GKMPT", "/Controls;component/Images/BMPT.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			_designerItems = new List<DesignerItem>();
			if (plan.ElementPolygonGKZones == null)
				plan.ElementPolygonGKZones = new List<ElementPolygonGKZone>();
			if (plan.ElementRectangleGKZones == null)
				plan.ElementRectangleGKZones = new List<ElementRectangleGKZone>();
			if (plan.ElementRectangleGKDelays == null)
				plan.ElementRectangleGKDelays = new List<ElementRectangleGKDelay>();
			if (plan.ElementPolygonGKDelays == null)
				plan.ElementPolygonGKDelays = new List<ElementPolygonGKDelay>();
			if (plan.ElementRectangleGKPumpStations == null)
				plan.ElementRectangleGKPumpStations = new List<ElementRectangleGKPumpStation>();
			if (plan.ElementPolygonGKPumpStations == null)
				plan.ElementPolygonGKPumpStations = new List<ElementPolygonGKPumpStation>();
			if (plan.ElementPolygonGKGuardZones == null)
				plan.ElementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
			if (plan.ElementRectangleGKGuardZones == null)
				plan.ElementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
			if (plan.ElementPolygonGKSKDZones == null)
				plan.ElementPolygonGKSKDZones = new List<ElementPolygonGKSKDZone>();
			if (plan.ElementRectangleGKSKDZones == null)
				plan.ElementRectangleGKSKDZones = new List<ElementRectangleGKSKDZone>();
			if (plan.ElementRectangleGKDirections == null)
				plan.ElementRectangleGKDirections = new List<ElementRectangleGKDirection>();
			if (plan.ElementPolygonGKDirections == null)
				plan.ElementPolygonGKDirections = new List<ElementPolygonGKDirection>();
			if (plan.ElementRectangleGKMPTs == null)
				plan.ElementRectangleGKMPTs = new List<ElementRectangleGKMPT>();
			if (plan.ElementPolygonGKMPTs == null)
				plan.ElementPolygonGKMPTs = new List<ElementPolygonGKMPT>();

			return new ElementBase[0]
				.Concat(plan.ElementGKDevices)
				.Concat(plan.ElementRectangleGKZones)
				.Concat(plan.ElementPolygonGKZones)
				.Concat(plan.ElementRectangleGKDelays)
				.Concat(plan.ElementPolygonGKDelays)
				.Concat(plan.ElementRectangleGKPumpStations)
				.Concat(plan.ElementPolygonGKPumpStations)
				.Concat(plan.ElementRectangleGKGuardZones)
				.Concat(plan.ElementPolygonGKGuardZones)
				.Concat(plan.ElementRectangleGKSKDZones)
				.Concat(plan.ElementPolygonGKSKDZones)
				.Concat(plan.ElementRectangleGKDirections)
				.Concat(plan.ElementPolygonGKDirections)
				.Concat(plan.ElementRectangleGKMPTs)
				.Concat(plan.ElementPolygonGKMPTs)
				.Concat(plan.ElementGKDoors)
				.ToArray();
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("GK", "Устройства", 12);
			LayerGroupService.Instance.RegisterGroup("GKZone", "Пожарные зоны", 13);
			LayerGroupService.Instance.RegisterGroup("GKGuardZone", "Охранные зоны", 14);
			LayerGroupService.Instance.RegisterGroup("GKSKDZone", "Зоны СКД", 15);
			LayerGroupService.Instance.RegisterGroup("GKDirection", "Направления", 16);
			LayerGroupService.Instance.RegisterGroup("GKMPT", "МПТ", 17);
			LayerGroupService.Instance.RegisterGroup("GKDoors", "Точки доступа", 18);
			LayerGroupService.Instance.RegisterGroup("GKDelay", "Задержки", 19);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("GKDevice.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		public override IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.NotBoundedElements))
				ClientManager.PlansConfiguration.AllPlans.ForEach(plan =>
				{
					errors.AddRange(FindUnbindedErrors<ElementGKDevice, ShowGKDeviceEvent, Guid>(plan.ElementGKDevices, plan.UID, "Несвязанное устройство", "/Controls;component/GKIcons/RM_1.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKZone, ShowGKZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKZones, plan.UID, "Несвязанная зона", "/Controls;component/Images/Zone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKZone, ShowGKZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKZones, plan.UID, "Несвязанная зона", "/Controls;component/Images/Zone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKDelay, ShowGKDelayEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKDelays, plan.UID, "Несвязанная задержка", "/Controls;component/Images/Delay.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKDelay, ShowGKDelayEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKDelays, plan.UID, "Несвязанная задержка", "/Controls;component/Images/Delay.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKPumpStation, ShowGKPumpStationOnPlanEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKPumpStations, plan.UID, "Несвязанная насосная станция", "/Controls;component/Images/PumpStation.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKPumpStation, ShowGKPumpStationOnPlanEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKPumpStations, plan.UID, "Несвязанная насосная станция", "/Controls;component/Images/PumpStation.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKGuardZone, ShowGKGuardZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKGuardZones, plan.UID, "Несвязанная охранная зона", "/Controls;component/Images/GuardZone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKGuardZone, ShowGKGuardZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKGuardZones, plan.UID, "Несвязанная охранная зона", "/Controls;component/Images/GuardZone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKSKDZone, ShowGKSKDZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKSKDZones, plan.UID, "Несвязанная зона СКД", "/Controls;component/Images/SKDZone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKSKDZone, ShowGKSKDZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKSKDZones, plan.UID, "Несвязанная зона СКД", "/Controls;component/Images/SKDZone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKDirection, ShowGKDirectionEvent, Guid>(plan.ElementRectangleGKDirections, plan.UID, "Несвязанное направление", "/Controls;component/Images/Blue_Direction.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKDirection, ShowGKDirectionEvent, Guid>(plan.ElementPolygonGKDirections, plan.UID, "Несвязанное направление", "/Controls;component/Images/Blue_Direction.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKMPT, ShowGKMPTEvent, Guid>(plan.ElementRectangleGKMPTs, plan.UID, "Несвязанный МПТ", "/Controls;component/Images/BMPT.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKMPT, ShowGKMPTEvent, Guid>(plan.ElementPolygonGKMPTs, plan.UID, "Несвязанный МПТ", "/Controls;component/Images/BMPT.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementGKDoor, ShowGKDoorEvent, ShowOnPlanArgs<Guid>>(plan.ElementGKDoors, plan.UID, "Несвязанное точка доступа", "/Controls;component/Images/Door.png", Guid.Empty));
				});
			return errors;
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(GKDevice))
			{
				var device = item as GKDevice;
				if (device == null)
				{
					designerItem.Title = "Неизвестное устройство";
					designerItem.IconSource = null;
				}
				else
				{
					designerItem.Title = device.PresentationName;
					designerItem.IconSource = device.Driver.ImageSource;
					var vizializationElement = designerItem.Element as IMultipleVizualization;
					if (vizializationElement != null)
						vizializationElement.AllowMultipleVizualization = device.AllowMultipleVizualization;
				}
			}
			else if (typeof(TItem) == typeof(GKZone))
			{
				var zone = item as GKZone;
				designerItem.Title = zone == null ? "Несвязанная зона" : zone.Name;
				designerItem.Index = zone == null ? default(int) : zone.No;
			}
			else if (typeof(TItem) == typeof(GKGuardZone))
			{
				var guardZone = item as GKGuardZone;
				designerItem.Title = guardZone == null ? "Неизвестная охраная зона" : guardZone.Name;
				designerItem.Index = guardZone == null ? default(int) : guardZone.No;
			}
			else if (typeof(TItem) == typeof(GKSKDZone))
			{
				var skdZone = item as GKSKDZone;
				designerItem.Title = skdZone == null ? "Неизвестная зона СКД" : skdZone.Name;
				designerItem.Index = skdZone == null ? default(int) : skdZone.No;
			}
			else if (typeof(TItem) == typeof(GKDirection))
			{
				var direction = item as GKDirection;
				designerItem.Title = direction == null ? "Несвязанное направление" : direction.Name;
				designerItem.Index = direction == null ? default(int) : direction.No;
			}
			else if (typeof(TItem) == typeof(GKMPT))
			{
				var mpt = item as GKMPT;
				designerItem.Title = mpt == null ? "Несвязанный МПТ" : mpt.Name;
				designerItem.Index = mpt == null ? default(int) : mpt.No;
			}
			else if (typeof(TItem) == typeof(GKDoor))
			{
				var door = item as GKDoor;
				designerItem.Title = door == null ? "Неизвестная точка доступа" : door.Name;
				designerItem.Index = door == null ? default(int) : door.No;
			}
			else if (typeof(TItem) == typeof(GKDelay))
			{
				var delay = item as GKDelay;
				designerItem.Title = delay == null ? "Неизвестная задержка" : delay.Name;
				designerItem.Index = delay == null ? default(int) : delay.No;
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}
		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(GKZone))
			{
				var elementZone = (IElementZone)element;
				elementZone.BackgroundColor = GetGKZoneColor(item as GKZone);
				elementZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(GKGuardZone))
			{
				var elementGuardZone = (IElementZone)element;
				elementGuardZone.BackgroundColor = GetGKGuardZoneColor(item as GKGuardZone);
				elementGuardZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(GKSKDZone))
			{
				var elementSKDZone = (IElementZone)element;
				elementSKDZone.BackgroundColor = GetGKSKDZoneColor(item as GKSKDZone);
				elementSKDZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(GKDirection))
			{
				var elementDirection = (IElementDirection)element;
				elementDirection.BackgroundColor = GetGKDirectionColor(item as GKDirection);
				elementDirection.SetZLayer(item == null ? 10 : 11);
			}
			else if (typeof(TItem) == typeof(GKMPT))
			{
				var elementMPT = (IElementMPT)element;
				elementMPT.BackgroundColor = GetGKMPTColor(item as GKMPT);
				elementMPT.SetZLayer(item == null ? 10 : 11);
			}
			else if (typeof(TItem) == typeof(GKDelay))
			{
				var elementDelay = (IElementDelay)element;
				elementDelay.BackgroundColor = GetGKDelayColor(item as GKDelay);
				elementDelay.SetZLayer(item == null ? 10 : 11);
			}
			else if (typeof(TItem) == typeof(GKPumpStation))
			{
				var elementPumpStation = (IElementPumpStation)element;
				elementPumpStation.BackgroundColor = GetGKPumpStationColor(item as GKPumpStation);
				elementPumpStation.SetZLayer(item == null ? 10 : 11);
			}
			else
				base.UpdateElementProperties<TItem>(element, item);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementGKDevice = args.Element as ElementGKDevice;
			if (elementGKDevice != null)
				args.Painter = new Painter(DesignerCanvas, elementGKDevice);
			else if (args.Element is ElementGKDoor)
				args.Painter = new GKDoorPainter(DesignerCanvas, (ElementGKDoor)args.Element);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementGKDevice element = e.Element as ElementGKDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleGKZone || e.Element is ElementPolygonGKZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
			else if (e.Element is ElementRectangleGKGuardZone || e.Element is ElementPolygonGKGuardZone)
				e.PropertyViewModel = new GuardZonePropertiesViewModel((IElementZone)e.Element, _guardZonesViewModel);
			else if (e.Element is ElementRectangleGKSKDZone || e.Element is ElementPolygonGKSKDZone)
				e.PropertyViewModel = new SKDZonePropertiesViewModel((IElementZone)e.Element, _skdZonesViewModel);
			else if (e.Element is ElementRectangleGKDirection || e.Element is ElementPolygonGKDirection)
				e.PropertyViewModel = new DirectionPropertiesViewModel((IElementDirection)e.Element, _directionsViewModel);
			else if (e.Element is ElementRectangleGKMPT || e.Element is ElementPolygonGKMPT)
				e.PropertyViewModel = new MPTPropertiesViewModel((IElementMPT)e.Element, _mptsViewModel);
			else if (e.Element is ElementRectangleGKDelay || e.Element is ElementPolygonGKDelay)
				e.PropertyViewModel = new DelayPropertiesViewModel((IElementDelay)e.Element, _delaysViewModel);
			else if (e.Element is ElementRectangleGKPumpStation || e.Element is ElementPolygonGKPumpStation)
				e.PropertyViewModel = new PumpStationPropertiesViewModel((IElementPumpStation)e.Element, _pumpStationsViewModel);
			else if (e.Element is ElementGKDoor)
				e.PropertyViewModel = new GKDoorPropertiesViewModel(_doorsViewModel, (ElementGKDoor)e.Element);
		}

		public void UpdateGKDeviceInGKZones(List<ElementBase> items)
		{
			if (IsDeviceInZonesChanged(items))
			{
				var deviceInZones = new Dictionary<GKDevice, Guid>();
				var handledDevices = new List<GKDevice>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateGKDeviceInZones: {0}"))
				{
					Dictionary<System.Windows.Media.Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in DesignerCanvas.Items)
					{
						var elementGKDevice = designerItem.Element as ElementGKDevice;
						if (elementGKDevice != null)
						{
							var device = GetItem<GKDevice>(elementGKDevice);
							if (device == null || device.Driver == null || handledDevices.Contains(device) || !device.Driver.HasZone)
								continue;
							var point = new Point(elementGKDevice.Left, elementGKDevice.Top);
							var zones = new List<IElementZone>();
							foreach (var pair in geometries)
								if (pair.Key.Bounds.Contains(point) && pair.Key.FillContains(point))
									zones.Add(pair.Value);
							switch (device.ZoneUIDs.Count)
							{
								case 0:
									if (zones.Count > 0)
									{
										var zone = GetItem<GKZone>(GetTopZoneUID(zones));
										if (zone != null)
										{
											GKManager.AddDeviceToZone(device, zone);
											handledDevices.Add(device);
										}
									}
									break;
								case 1:
									var isInZone = zones.Any(x => x.ZoneUID == device.ZoneUIDs[0]);
									if (!isInZone)
									{
										if (!deviceInZones.ContainsKey(device) && !device.AllowBeOutsideZone)
											deviceInZones.Add(device, GetTopZoneUID(zones));
									}
									else
									{
										handledDevices.Add(device);
										if (deviceInZones.ContainsKey(device))
											deviceInZones.Remove(device);
										device.AllowBeOutsideZone = false;
									}
									break;
							}
						}
					}
				}
				ShowDeviceInZoneChanged(deviceInZones);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}
		private bool IsDeviceInZonesChanged(List<ElementBase> items)
		{
			if (_processChanges && !DesignerCanvas.IsLocked)
				foreach (var item in items)
					if (item is ElementGKDevice || item is IElementZone)
						return true;
			return false;
		}
		private Dictionary<System.Windows.Media.Geometry, IElementZone> GetZoneGeometryMap()
		{
			var geometries = new Dictionary<System.Windows.Media.Geometry, IElementZone>();
			foreach (var designerItem in DesignerCanvas.Items)
			{
				var elementZone = designerItem.Element as IElementZone;
				if (elementZone != null && elementZone.ZoneUID != Guid.Empty)
					geometries.Add(((IGeometryPainter)designerItem.Painter).Geometry, elementZone);
			}
			return geometries;
		}
		private Guid GetTopZoneUID(List<IElementZone> zones)
		{
			return zones.OrderByDescending(item => item.ZIndex).Select(item => item.ZoneUID).FirstOrDefault();
		}
		private void ShowDeviceInZoneChanged(Dictionary<GKDevice, Guid> deviceInZones)
		{
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
				if (!result)
				{
					_processChanges = false;
					if (DesignerCanvas != null)
						DesignerCanvas.RevertLastAction();
					_processChanges = true;
				}
			}
		}

		public void InvalidateCanvas()
		{
			_designerItems.ForEach(item =>
			{
				OnDesignerItemPropertyChanged<GKDevice>(item);
				UpdateProperties<GKDevice>(item);
				item.Painter.Invalidate();
			});
			if (DesignerCanvas != null)
				DesignerCanvas.Refresh();
		}

		private Color GetGKDirectionColor(GKDirection direction)
		{
			Color color = Colors.Black;
			if (direction != null)
				color = Colors.LightBlue;
			return color;
		}
		private Color GetGKMPTColor(GKMPT mpt)
		{
			Color color = Colors.Black;
			if (mpt != null)
				color = Colors.LightBlue;
			return color;
		}
		private Color GetGKGuardZoneColor(GKGuardZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Brown;
			return color;
		}
		private Color GetGKZoneColor(GKZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
		private Color GetGKSKDZoneColor(GKSKDZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
		private Color GetGKDelayColor(GKDelay delay)
		{
			Color color = Colors.Black;
			if (delay != null)
				color = Colors.LightBlue;
			return color;
		}
		private Color GetGKPumpStationColor(GKPumpStation pumpStation)
		{
			Color color = Colors.Black;
			if (pumpStation != null)
				color = Colors.Cyan;
			return color;
		}
	}
}