using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
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

namespace GKModule.Plans
{
	class GKPlanExtension : BasePlanExtension
	{
		public static GKPlanExtension Instance { get; private set; }

		private bool _processChanges;
		private DevicesViewModel _devicesViewModel;
		private SKDZonesViewModel _skdZonesViewModel;
		private DoorsViewModel _doorsViewModel;
		private IEnumerable<IInstrument> _instruments;
		private List<DesignerItem> _designerItems;

		public GKPlanExtension(DevicesViewModel devicesViewModel, SKDZonesViewModel skdZonesViewModel, DoorsViewModel doorsViewModel)
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
			_skdZonesViewModel = skdZonesViewModel;
			_doorsViewModel = doorsViewModel;
			_instruments = null;
			_processChanges = true;
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKSKDZone>(() => GKManager.SKDZones);
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
				if (element is ElementRectangleGKSKDZone)
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
				if (element is ElementRectangleGKSKDZone)
					plan.ElementRectangleGKSKDZones.Remove((ElementRectangleGKSKDZone)element);
				else if (element is ElementPolygonGKSKDZone)
					plan.ElementPolygonGKSKDZones.Remove((ElementPolygonGKSKDZone)element);
				else
					return false;
				ResetItem<GKSKDZone>((IElementZone)element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleGKSKDZone || designerItem.Element is ElementPolygonGKSKDZone)
				RegisterDesignerItem<GKSKDZone>(designerItem, "GKSKDZone", "/Controls;component/Images/SKDZone.png");
			else if (designerItem.Element is ElementGKDoor)
				RegisterDesignerItem<GKDoor>(designerItem, "GKDoors", "/Controls;component/Images/Door.png");
			else if (designerItem.Element is ElementGKDevice)
			{
				RegisterDesignerItem<GKDevice>(designerItem, "GK");
				_designerItems.Add(designerItem);
			}
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			_designerItems = new List<DesignerItem>();
			if (plan.ElementPolygonGKSKDZones == null)
				plan.ElementPolygonGKSKDZones = new List<ElementPolygonGKSKDZone>();
			if (plan.ElementRectangleGKSKDZones == null)
				plan.ElementRectangleGKSKDZones = new List<ElementRectangleGKSKDZone>();
			foreach (var element in plan.ElementGKDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleGKSKDZones)
				yield return element;
			foreach (var element in plan.ElementPolygonGKSKDZones)
				yield return element;
			foreach (var element in plan.ElementGKDoors)
				yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("GK", "Устройства", 12);
			LayerGroupService.Instance.RegisterGroup("GKSKDZone", "Зоны СКД", 15);
			LayerGroupService.Instance.RegisterGroup("GKDoors", "ГК Точки доступа", 18);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("GKDevice.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		public override IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			if (GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.NotBoundedElements))
				FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
				{
					errors.AddRange(FindUnbindedErrors<ElementGKDevice, ShowGKDeviceEvent, Guid>(plan.ElementGKDevices, plan.UID, "Несвязанное устройство", "/Controls;component/GKIcons/RM_1.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementRectangleGKSKDZone, ShowGKSKDZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementRectangleGKSKDZones, plan.UID, "Несвязанная зона СКД", "/Controls;component/Images/SKDZone.png", Guid.Empty));
					errors.AddRange(FindUnbindedErrors<ElementPolygonGKSKDZone, ShowGKSKDZoneEvent, ShowOnPlanArgs<Guid>>(plan.ElementPolygonGKSKDZones, plan.UID, "Несвязанная зона СКД", "/Controls;component/Images/SKDZone.png", Guid.Empty));
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
				designerItem.Title = device == null ? "Неизвестное устройство" : device.PresentationName;
				designerItem.IconSource = device == null ? null : device.Driver.ImageSource;
			}
			else if (typeof(TItem) == typeof(GKSKDZone))
			{
				var skdZone = item as GKSKDZone;
				designerItem.Title = skdZone == null ? "Неизвестная зона СКД" : skdZone.Name;
				designerItem.Index = skdZone == null ? default(int) : skdZone.No;
			}
			else if (typeof(TItem) == typeof(GKDoor))
			{
				var door = item as GKDoor;
				designerItem.Title = door == null ? "Неизвестная точка доступа" : door.Name;
				designerItem.Index = door == null ? default(int) : door.No;
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}
		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(GKSKDZone))
			{
				var elementSKDZone = (IElementZone)element;
				elementSKDZone.BackgroundColor = GetGKSKDZoneColor(item as GKSKDZone);
				elementSKDZone.SetZLayer(item == null ? 50 : 60);
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
			else if (e.Element is ElementRectangleGKSKDZone || e.Element is ElementPolygonGKSKDZone)
				e.PropertyViewModel = new SKDZonePropertiesViewModel((IElementZone)e.Element, _skdZonesViewModel);
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
				using (new TimeCounter("\tUpdateGKDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in DesignerCanvas.Items)
					{
						var elementGKDevice = designerItem.Element as ElementGKDevice;
						if (elementGKDevice != null)
						{
							var device = GetItem<GKDevice>(elementGKDevice);
							if (device == null || device.Driver == null || handledXDevices.Contains(device))
								continue;
							var point = new Point(elementGKDevice.Left, elementGKDevice.Top);
							var zones = new List<IElementZone>();
							foreach (var pair in geometries)
								if (pair.Key.Bounds.Contains(point) && pair.Key.FillContains(point))
									zones.Add(pair.Value);
							switch (device.ZoneUIDs.Count)
							{
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
		private Color GetGKSKDZoneColor(GKSKDZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
	}
}