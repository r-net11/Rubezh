using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using GKModule.Plans.InstrumentAdorners;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Interfaces;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Services;

namespace GKModule.Plans
{
	class GKPlanExtension : BasePlanExtension
	{
		public static GKPlanExtension Instance { get; private set; }

		private bool _processChanges;
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DirectionsViewModel _directionsViewModel;
		private GuardZonesViewModel _guardZonesViewModel;
		private DoorsViewModel _doorsViewModel;
		private IEnumerable<IInstrument> _instruments;
		private List<DesignerItem> _designerItems;

		public GKPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, DirectionsViewModel directionsViewModel, GuardZonesViewModel guardZonesViewModel, DoorsViewModel doorsViewModel)
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
			_directionsViewModel = directionsViewModel;
			_guardZonesViewModel = guardZonesViewModel;
			_doorsViewModel = doorsViewModel;
			_instruments = null;
			_processChanges = true;
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKZone>(() => GKManager.Zones);
			Cache.Add<GKGuardZone>(() => GKManager.DeviceConfiguration.GuardZones);
			Cache.Add<GKDirection>(() => GKManager.Directions);
			Cache.Add<GKDoor>(() => GKManager.Doors);
			_designerItems = new List<DesignerItem>();
		}

		public override void Initialize()
		{
			base.Initialize();
			using (new TimeCounter("DevicePictureCache.LoadGKCache: {0}"))
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
							ImageSource="/Controls;component/Images/ZoneRectangle.png",
							ToolTip="Зона",
							Adorner = new XZoneRectangleAdorner(DesignerCanvas, _zonesViewModel),
							Index = 200,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="Зона",
							Adorner = new XZonePolygonAdorner(DesignerCanvas, _zonesViewModel),
							Index = 201,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/DirectionRectangle.png",
							ToolTip="Направление",
							Adorner = new XDirectionRectangleAdorner(DesignerCanvas, _directionsViewModel),
							Index = 202,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/DirectionPolygon.png",
							ToolTip="Направление",
							Adorner = new XDirectionPolygonAdorner(DesignerCanvas, _directionsViewModel),
							Index = 203,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZoneRectangle.png",
							ToolTip="Охранная зона",
							Adorner = new XGuardZoneRectangleAdorner(DesignerCanvas, _guardZonesViewModel),
							Index = 204,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="Охранная зона",
							Adorner = new XGuardZonePolygonAdorner(DesignerCanvas,  _guardZonesViewModel),
							Index = 205,
							Autostart = true
						},
	};
				return _instruments;
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementGKDevice)
			{
				var elementXDevice = element as ElementGKDevice;
				plan.ElementGKDevices.Add(elementXDevice);
				SetItem<GKDevice>(elementXDevice);
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
					var elementRectangleXZone = (ElementRectangleGKZone)element;
					plan.ElementRectangleGKZones.Add(elementRectangleXZone);
					SetItem<GKZone>(elementRectangleXZone);
				}
				else if (element is ElementPolygonGKZone)
				{
					var elementPolygonXZone = (ElementPolygonGKZone)element;
					plan.ElementPolygonGKZones.Add(elementPolygonXZone);
					SetItem<GKZone>(elementPolygonXZone);
				}
				else if (element is ElementRectangleGKGuardZone)
				{
					var elementRectangleXGuardZone = (ElementRectangleGKGuardZone)element;
					plan.ElementRectangleGKGuardZones.Add(elementRectangleXGuardZone);
					SetItem<GKGuardZone>(elementRectangleXGuardZone);
				}
				else if (element is ElementPolygonGKGuardZone)
				{
					var elementPolygonXGuardZone = (ElementPolygonGKGuardZone)element;
					plan.ElementPolygonGKGuardZones.Add(elementPolygonXGuardZone);
					SetItem<GKGuardZone>(elementPolygonXGuardZone);
				}
				else
					return false;
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
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementGKDevice)
			{
				var elementXDevice = (ElementGKDevice)element;
				plan.ElementGKDevices.Remove(elementXDevice);
				ResetItem<GKDevice>(elementXDevice);
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
				else
					return false;
				ResetItem<GKZone>((IElementZone)element);
				ResetItem<GKGuardZone>((IElementZone)element);
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
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleGKZone || designerItem.Element is ElementPolygonGKZone)
				RegisterDesignerItem<GKZone>(designerItem, "GKZone", "/Controls;component/Images/Zone.png");
			else if (designerItem.Element is ElementRectangleGKGuardZone || designerItem.Element is ElementPolygonGKGuardZone)
				RegisterDesignerItem<GKGuardZone>(designerItem, "GKGuardZone", "/Controls;component/Images/GuardZone.png");
			else if (designerItem.Element is ElementGKDoor)
				RegisterDesignerItem<GKDoor>(designerItem, "GKDoors", "/Controls;component/Images/Door.png");
			else if (designerItem.Element is ElementGKDevice)
			{
				RegisterDesignerItem<GKDevice>(designerItem, "GK");
				_designerItems.Add(designerItem);
			}
			else if (designerItem.Element is IElementDirection)
				RegisterDesignerItem<GKDirection>(designerItem, "GKDirection", "/Controls;component/Images/Blue_Direction.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			_designerItems = new List<DesignerItem>();
			if (plan.ElementPolygonGKZones == null)
				plan.ElementPolygonGKZones = new List<ElementPolygonGKZone>();
			if (plan.ElementRectangleGKZones == null)
				plan.ElementRectangleGKZones = new List<ElementRectangleGKZone>();
			if (plan.ElementPolygonGKGuardZones == null)
				plan.ElementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
			if (plan.ElementRectangleGKGuardZones == null)
				plan.ElementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
			if (plan.ElementRectangleGKDirections == null)
				plan.ElementRectangleGKDirections = new List<ElementRectangleGKDirection>();
			if (plan.ElementPolygonGKDirections == null)
				plan.ElementPolygonGKDirections = new List<ElementPolygonGKDirection>();
			foreach (var element in plan.ElementGKDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleGKZones)
				yield return element;
			foreach (var element in plan.ElementPolygonGKZones)
				yield return element;
			foreach (var element in plan.ElementRectangleGKGuardZones)
				yield return element;
			foreach (var element in plan.ElementPolygonGKGuardZones)
				yield return element;
			foreach (var element in plan.ElementRectangleGKDirections)
				yield return element;
			foreach (var element in plan.ElementPolygonGKDirections)
				yield return element;
			foreach (var element in plan.ElementGKDoors)
				yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("GK", "Устройства", 12);
			LayerGroupService.Instance.RegisterGroup("GKZone", "Зоны", 13);
			LayerGroupService.Instance.RegisterGroup("GKDirection", "Направления", 14);
			LayerGroupService.Instance.RegisterGroup("GKGuardZone", "Охранные зоны", 15);
			LayerGroupService.Instance.RegisterGroup("GKDoors", "ГК Точки доступа", 16);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("GKDevice.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(GKDevice))
			{
				var device = item as GKDevice;
				designerItem.Title = device == null ? "Неизвестное устройство" : device.PresentationName;
				designerItem.IconSource = device == null ? null : device.Driver.ImageSource;
			}
			else if (typeof(TItem) == typeof(GKZone))
			{
				var zone = item as GKZone;
				designerItem.Title = zone == null ? "Несвязанная зона" : zone.PresentationName;
			}
			else if (typeof(TItem) == typeof(GKGuardZone))
			{
				var guardZone = item as GKGuardZone;
				designerItem.Title = guardZone == null ? "Неизвестная охраная зона" : guardZone.PresentationName;
			}
			else if (typeof(TItem) == typeof(GKDirection))
			{
				var direction = item as GKDirection;
				designerItem.Title = direction == null ? "Несвязанное направление" : direction.PresentationName;
			}
			else if (typeof(TItem) == typeof(GKDoor))
			{
				var door = item as GKDoor;
				designerItem.Title = door == null ? "Неизвестная точка доступа" : door.PresentationName;
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}
		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(GKZone))
			{
				var elementZone = (IElementZone)element;
				elementZone.BackgroundColor = GetXZoneColor(item as GKZone);
				elementZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(GKGuardZone))
			{
				var elementGuardZone = (IElementZone)element;
				elementGuardZone.BackgroundColor = GetXGuardZoneColor(item as GKGuardZone);
				elementGuardZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(GKDirection))
			{
				var elementDirection = (IElementDirection)element;
				elementDirection.BackgroundColor = GetXDirectionColor(item as GKDirection);
				elementDirection.SetZLayer(item == null ? 10 : 11);
			}
			else
				base.UpdateElementProperties<TItem>(element, item);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementXDevice = args.Element as ElementGKDevice;
			if (elementXDevice != null)
				args.Painter = new Painter(DesignerCanvas, elementXDevice);
			else if (args.Element is ElementGKDoor)
				args.Painter = new GKDoorPainter(DesignerCanvas, (ElementGKDoor)args.Element);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementGKDevice element = e.Element as ElementGKDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleGKZone || e.Element is ElementPolygonGKZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel, (ElementBase)e.Element);
			else if (e.Element is ElementRectangleGKGuardZone || e.Element is ElementPolygonGKGuardZone)
				e.PropertyViewModel = new GuardZonePropertiesViewModel((IElementZone)e.Element, _guardZonesViewModel, (ElementBase)e.Element);
			else if (e.Element is ElementRectangleGKDirection || e.Element is ElementPolygonGKDirection)
				e.PropertyViewModel = new DirectionPropertiesViewModel((IElementDirection)e.Element, _directionsViewModel, (ElementBase)e.Element);
			else if (e.Element is ElementGKDoor)
				e.PropertyViewModel = new GKDoorPropertiesViewModel(_doorsViewModel, (ElementGKDoor)e.Element);
		}

		public void UpdateGKDeviceInGKZones(List<ElementBase> items)
		{
			if (IsDeviceInZonesChanged(items))
			{
				var deviceInZones = new Dictionary<GKDevice, Guid>();
				var handledXDevices = new List<GKDevice>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateXDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in DesignerCanvas.Items)
					{
						var elementXDevice = designerItem.Element as ElementGKDevice;
						if (elementXDevice != null)
						{
							var device = GetItem<GKDevice>(elementXDevice);
							if (device == null || device.Driver == null || handledXDevices.Contains(device))
								continue;
							var point = new Point(elementXDevice.Left, elementXDevice.Top);
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
											handledXDevices.Add(device);
										}
									}
									break;
								case 1:
									var isInZone = zones.Any(x => x.ZoneUID == device.ZoneUIDs[0]);
									if (!isInZone)
									{
										if (!deviceInZones.ContainsKey(device))
											deviceInZones.Add(device, GetTopZoneUID(zones));
									}
									else
									{
										handledXDevices.Add(device);
										if (deviceInZones.ContainsKey(device))
											deviceInZones.Remove(device);
									}
									break;
							}
						}
					}
				}
				ShowDeviceInZoneChanged(deviceInZones);
			}
		}
		private bool IsDeviceInZonesChanged(List<ElementBase> items)
		{
			if (_processChanges && !DesignerCanvas.IsLocked)
				foreach (var item in items)
					if (item is ElementGKDevice || item is IElementZone)
						return true;
			return false;
		}
		private Dictionary<Geometry, IElementZone> GetZoneGeometryMap()
		{
			var geometries = new Dictionary<Geometry, IElementZone>();
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
			DesignerCanvas.Refresh();
		}

		private Color GetXDirectionColor(GKDirection direction)
		{
			Color color = Colors.Black;
			if (direction != null)
				color = Colors.LightBlue;
			return color;
		}
		private Color GetXGuardZoneColor(GKGuardZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Brown;
			return color;
		}
		private Color GetXZoneColor(GKZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
	}
}