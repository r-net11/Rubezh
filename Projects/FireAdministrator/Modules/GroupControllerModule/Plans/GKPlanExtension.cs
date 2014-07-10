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
		private IEnumerable<IInstrument> _instruments;
		private List<DesignerItem> _designerItems;

		public GKPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, DirectionsViewModel directionsViewModel, GuardZonesViewModel guardZonesViewModel)
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(UpdateXDeviceInXZones);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(UpdateXDeviceInXZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(UpdateXDeviceInXZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(UpdateXDeviceInXZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateXDeviceInXZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateXDeviceInXZones);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_directionsViewModel = directionsViewModel;
			_guardZonesViewModel = guardZonesViewModel;
			_instruments = null;
			_processChanges = true;
			Cache.Add<XDevice>(() => XManager.Devices);
			Cache.Add<XZone>(() => XManager.Zones);
			Cache.Add<XGuardZone>(() => XManager.DeviceConfiguration.GuardZones);
			Cache.Add<XDirection>(() => XManager.Directions);
			_designerItems = new List<DesignerItem>();
		}

		public void Initialize()
		{
			Cache.BuildAllSafe();
			using (new TimeCounter("DevicePictureCache.LoadXCache: {0}"))
				PictureCacheSource.XDevicePicture.LoadCache();
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
			if (element is ElementXDevice)
			{
				var elementXDevice = element as ElementXDevice;
				plan.ElementXDevices.Add(elementXDevice);
				SetItem<XDevice>(elementXDevice);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleXZone)
				{
					var elementRectangleXZone = (ElementRectangleXZone)element;
					plan.ElementRectangleXZones.Add(elementRectangleXZone);
					SetItem<XZone>(elementRectangleXZone);
				}
				else if (element is ElementPolygonXZone)
				{
					var elementPolygonXZone = (ElementPolygonXZone)element;
					plan.ElementPolygonXZones.Add(elementPolygonXZone);
					SetItem<XZone>(elementPolygonXZone);
				}
				else if (element is ElementRectangleXGuardZone)
				{
					var elementRectangleXGuardZone = (ElementRectangleXGuardZone)element;
					plan.ElementRectangleXGuardZones.Add(elementRectangleXGuardZone);
					SetItem<XGuardZone>(elementRectangleXGuardZone);
				}
				else if (element is ElementPolygonXGuardZone)
				{
					var elementPolygonXGuardZone = (ElementPolygonXGuardZone)element;
					plan.ElementPolygonXGuardZones.Add(elementPolygonXGuardZone);
					SetItem<XGuardZone>(elementPolygonXGuardZone);
				}
				else
					return false;
				return true;
			}
			else if (element is IElementDirection)
			{
				if (element is ElementRectangleXDirection)
					plan.ElementRectangleXDirections.Add((ElementRectangleXDirection)element);
				else if (element is ElementPolygonXDirection)
					plan.ElementPolygonXDirections.Add((ElementPolygonXDirection)element);
				else
					return false;
				SetItem<XDirection>((IElementDirection)element);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementXDevice)
			{
				var elementXDevice = (ElementXDevice)element;
				plan.ElementXDevices.Remove(elementXDevice);
				ResetItem<XDevice>(elementXDevice);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleXZone)
					plan.ElementRectangleXZones.Remove((ElementRectangleXZone)element);
				else if (element is ElementPolygonXZone)
					plan.ElementPolygonXZones.Remove((ElementPolygonXZone)element);
				else if (element is ElementRectangleXGuardZone)
					plan.ElementRectangleXGuardZones.Remove((ElementRectangleXGuardZone)element);
				else if (element is ElementPolygonXGuardZone)
					plan.ElementPolygonXGuardZones.Remove((ElementPolygonXGuardZone)element);
				else
					return false;
				ResetItem<XZone>((IElementZone)element);
				ResetItem<XGuardZone>((IElementZone)element);
				return true;
			}
			else if (element is IElementDirection)
			{
				if (element is ElementRectangleXDirection)
					plan.ElementRectangleXDirections.Remove((ElementRectangleXDirection)element);
				else if (element is ElementPolygonXDirection)
					plan.ElementPolygonXDirections.Remove((ElementPolygonXDirection)element);
				else
					return false;
				ResetItem<XDirection>((IElementDirection)element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleXZone || designerItem.Element is ElementPolygonXZone)
				RegisterDesignerItem<XZone>(designerItem, "XZone", "/Controls;component/Images/zone.png");
			else if (designerItem.Element is ElementRectangleXGuardZone || designerItem.Element is ElementPolygonXGuardZone)
				RegisterDesignerItem<XGuardZone>(designerItem, "XGuardZone", "/Controls;component/Images/zone.png");
			else if (designerItem.Element is ElementXDevice)
			{
				RegisterDesignerItem<XDevice>(designerItem, "GK");
				_designerItems.Add(designerItem);
			}
			else if (designerItem.Element is IElementDirection)
				RegisterDesignerItem<XDirection>(designerItem, "XDirection", "/Controls;component/Images/Blue_Direction.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			_designerItems = new List<DesignerItem>();
			if (plan.ElementPolygonXZones == null)
				plan.ElementPolygonXZones = new List<ElementPolygonXZone>();
			if (plan.ElementRectangleXZones == null)
				plan.ElementRectangleXZones = new List<ElementRectangleXZone>();
			if (plan.ElementPolygonXGuardZones == null)
				plan.ElementPolygonXGuardZones = new List<ElementPolygonXGuardZone>();
			if (plan.ElementRectangleXGuardZones == null)
				plan.ElementRectangleXGuardZones = new List<ElementRectangleXGuardZone>();
			if (plan.ElementRectangleXDirections == null)
				plan.ElementRectangleXDirections = new List<ElementRectangleXDirection>();
			if (plan.ElementPolygonXDirections == null)
				plan.ElementPolygonXDirections = new List<ElementPolygonXDirection>();
			foreach (var element in plan.ElementXDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleXZones)
				yield return element;
			foreach (var element in plan.ElementPolygonXZones)
				yield return element;
			foreach (var element in plan.ElementRectangleXGuardZones)
				yield return element;
			foreach (var element in plan.ElementPolygonXGuardZones)
				yield return element;
			foreach (var element in plan.ElementRectangleXDirections)
				yield return element;
			foreach (var element in plan.ElementPolygonXDirections)
				yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("GK", "Устройства", 2);
			LayerGroupService.Instance.RegisterGroup("XZone", "Зоны", 3);
			LayerGroupService.Instance.RegisterGroup("XDirection", "Направления", 4);
			LayerGroupService.Instance.RegisterGroup("XGuardZone", "Охранные зоны", 5);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(XDevice))
			{
				var device = item as XDevice;
				designerItem.Title = device == null ? "Неизвестное устройство" : device.PresentationName;
				designerItem.IconSource = device == null ? null : device.Driver.ImageSource;
			}
			else if (typeof(TItem) == typeof(XZone))
			{
				var zone = item as XZone;
				designerItem.Title = zone == null ? "Несвязанная зона" : zone.PresentationName;
			}
			else if (typeof(TItem) == typeof(XGuardZone))
			{
				var guardZone = item as XGuardZone;
				designerItem.Title = guardZone == null ? "Неизвестная дверь" : guardZone.PresentationName;
			}
			else if (typeof(TItem) == typeof(XDirection))
			{
				var direction = item as XDirection;
				designerItem.Title = direction == null ? "Несвязанное направление" : direction.PresentationName;
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}
		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(XZone))
			{
				var elementZone = (IElementZone)element;
				elementZone.BackgroundColor = GetXZoneColor(item as XZone);
				elementZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(XGuardZone))
			{
				var elementGuardZone = (IElementZone)element;
				elementGuardZone.BackgroundColor = GetXGuardZoneColor(item as XGuardZone);
				elementGuardZone.SetZLayer(item == null ? 50 : 60);
			}
			else if (typeof(TItem) == typeof(XDirection))
			{
				var elementDirection = (IElementDirection)element;
				elementDirection.BackgroundColor = GetXDirectionColor(item as XDirection);
				elementDirection.SetZLayer(item == null ? 10 : 11);
			}
			else
				base.UpdateElementProperties<TItem>(element, item);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementXDevice = args.Element as ElementXDevice;
			if (elementXDevice != null)
				args.Painter = new Painter(DesignerCanvas, elementXDevice);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementXDevice element = e.Element as ElementXDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleXZone || e.Element is ElementPolygonXZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
			else if (e.Element is ElementRectangleXGuardZone || e.Element is ElementPolygonXGuardZone)
				e.PropertyViewModel = new GuardZonePropertiesViewModel((IElementZone)e.Element, _guardZonesViewModel);
			else if (e.Element is ElementRectangleXDirection || e.Element is ElementPolygonXDirection)
				e.PropertyViewModel = new DirectionPropertiesViewModel((IElementDirection)e.Element, _directionsViewModel);
		}

		public void UpdateXDeviceInXZones(List<ElementBase> items)
		{
			if (IsDeviceInZonesChanged(items))
			{
				var deviceInZones = new Dictionary<XDevice, Guid>();
				var handledXDevices = new List<XDevice>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateXDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in DesignerCanvas.Items)
					{
						var elementXDevice = designerItem.Element as ElementXDevice;
						if (elementXDevice != null)
						{
							var xdevice = GetItem<XDevice>(elementXDevice);
							if (xdevice == null || xdevice.Driver == null || handledXDevices.Contains(xdevice))
								continue;
							var point = new Point(elementXDevice.Left, elementXDevice.Top);
							var zones = new List<IElementZone>();
							foreach (var pair in geometries)
								if (pair.Key.Bounds.Contains(point) && pair.Key.FillContains(point))
									zones.Add(pair.Value);
							switch (xdevice.ZoneUIDs.Count)
							{
								case 0:
									if (zones.Count > 0)
									{
										var zone = GetItem<XZone>(GetTopZoneUID(zones));
										if (zone != null)
										{
											XManager.AddDeviceToZone(xdevice, zone);
											handledXDevices.Add(xdevice);
										}
									}
									break;
								case 1:
									var isInZone = zones.Any(x => x.ZoneUID == xdevice.ZoneUIDs[0]);
									if (!isInZone)
									{
										if (!deviceInZones.ContainsKey(xdevice))
											deviceInZones.Add(xdevice, GetTopZoneUID(zones));
									}
									else
									{
										handledXDevices.Add(xdevice);
										if (deviceInZones.ContainsKey(xdevice))
											deviceInZones.Remove(xdevice);
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
					if (item is ElementXDevice || item is IElementZone)
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
		private void ShowDeviceInZoneChanged(Dictionary<XDevice, Guid> deviceInZones)
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
				OnDesignerItemPropertyChanged<XDevice>(item);
				UpdateProperties<XDevice>(item);
				item.Painter.Invalidate();
			});
			DesignerCanvas.Refresh();
		}

		private Color GetXDirectionColor(XDirection direction)
		{
			Color color = Colors.Black;
			if (direction != null)
				color = Colors.LightBlue;
			return color;
		}
		private Color GetXGuardZoneColor(XGuardZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Brown;
			return color;
		}
		private Color GetXZoneColor(XZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}

	}
}