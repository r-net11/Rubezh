using System.Collections.Generic;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using SKDModule.Plans.Designer;
using SKDModule.Plans.InstrumentAdorners;
using SKDModule.Plans.ViewModels;
using SKDModule.ViewModels;
using Infrustructure.Plans.Interfaces;

namespace SKDModule.Plans
{
	class SKDPlanExtension : BasePlanExtension
	{
		public static SKDPlanExtension Instance { get; private set; }

		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DoorsViewModel _doorsViewModel;
		private IEnumerable<IInstrument> _instruments;

		public SKDPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, DoorsViewModel doorsViewModel)
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_doorsViewModel = doorsViewModel;
			_instruments = null;
			Cache.Add<SKDDevice>(() => SKDManager.Devices);
			Cache.Add<SKDZone>(() => SKDManager.Zones);
			Cache.Add<SKDDoor>(() => SKDManager.SKDConfiguration.Doors);
		}

		public void Initialize()
		{
			Cache.BuildAllSafe();
			using (new TimeCounter("DevicePictureCache.LoadSKDCache: {0}"))
				PictureCacheSource.SKDDevicePicture.LoadCache();
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return "СКД Устройства"; }
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
							ToolTip="СКД Зона",
							Adorner = new SKDZoneRectangleAdorner(DesignerCanvas, _zonesViewModel),
							Index = 300,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="СКД Зона",
							Adorner = new SKDZonePolygonAdorner(DesignerCanvas, _zonesViewModel),
							Index = 301,
							Autostart = true
						},
					};
				return _instruments;
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementSKDDevice)
			{
				var elementSKDDevice = (ElementSKDDevice)element;
				plan.ElementSKDDevices.Add(elementSKDDevice);
				SetItem<SKDDevice>(elementSKDDevice);
				return true;
			}
			else if (element is ElementDoor)
			{
				var elementDoor = (ElementDoor)element;
				plan.ElementDoors.Add(elementDoor);
				SetItem<SKDDoor>(elementDoor);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleSKDZone)
					plan.ElementRectangleSKDZones.Add((ElementRectangleSKDZone)element);
				else if (element is ElementPolygonSKDZone)
					plan.ElementPolygonSKDZones.Add((ElementPolygonSKDZone)element);
				else
					return false;
				SetItem<SKDZone>((IElementZone)element);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementSKDDevice)
			{
				var elementSKDDevice = (ElementSKDDevice)element;
				plan.ElementSKDDevices.Remove(elementSKDDevice);
				ResetItem<SKDDevice>(elementSKDDevice);
				return true;
			}
			else if (element is ElementDoor)
			{
				var elementDoor = (ElementDoor)element;
				plan.ElementDoors.Remove(elementDoor);
				ResetItem<SKDDoor>(elementDoor);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleSKDZone)
					plan.ElementRectangleSKDZones.Remove((ElementRectangleSKDZone)element);
				else if (element is ElementPolygonSKDZone)
					plan.ElementPolygonSKDZones.Remove((ElementPolygonSKDZone)element);
				else
					return false;
				ResetItem<SKDZone>((IElementZone)element);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementSKDDevice)
				RegisterDesignerItem<SKDDevice>(designerItem, "SKD");
			else if (designerItem.Element is ElementRectangleSKDZone || designerItem.Element is ElementPolygonSKDZone)
				RegisterDesignerItem<SKDZone>(designerItem, "SKDZone", "/Controls;component/Images/Zone.png");
			else if (designerItem.Element is ElementDoor)
				RegisterDesignerItem<SKDDoor>(designerItem, "Doors", "/Controls;component/Images/Door.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementPolygonSKDZones == null)
				plan.ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
			if (plan.ElementRectangleSKDZones == null)
				plan.ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
			foreach (var element in plan.ElementSKDDevices)
				yield return element;
			foreach (var element in plan.ElementDoors)
				yield return element;
			foreach (var element in plan.ElementRectangleSKDZones)
				yield return element;
			foreach (var element in plan.ElementPolygonSKDZones)
				yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("SKD", "СКД Устройства", 5);
			LayerGroupService.Instance.RegisterGroup("SKDZone", "СКД Зоны", 6);
			LayerGroupService.Instance.RegisterGroup("Doors", "Точки доступа", 7);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		#endregion

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementSKDDevice = args.Element as ElementSKDDevice;
			if (elementSKDDevice != null)
				args.Painter = new Painter(DesignerCanvas, elementSKDDevice);
			else if (args.Element is ElementDoor)
				args.Painter = new DoorPainter(DesignerCanvas, (ElementDoor)args.Element);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementSKDDevice element = e.Element as ElementSKDDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementDoor)
				e.PropertyViewModel = new DoorPropertiesViewModel(_doorsViewModel, (ElementDoor)e.Element);
			else if (e.Element is ElementRectangleSKDZone || e.Element is ElementPolygonSKDZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
		}

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(SKDDevice))
			{
				var device = item as SKDDevice;
				designerItem.Title = device == null ? "Неизвестное устройство" : device.Name;
				designerItem.IconSource = device == null ? null : device.Driver.ImageSource;
			}
			else if (typeof(TItem) == typeof(SKDZone))
			{
				var zone = item as SKDZone;
				designerItem.Title = zone == null ? "Несвязанная зона" : zone.Name;
			}
			else if (typeof(TItem) == typeof(SKDDoor))
			{
				var door = item as SKDDoor;
				designerItem.Title = door == null ? "Неизвестная дверь" : door.Name;
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}
		protected override void UpdateElementProperties<TItem>(IElementReference element, TItem item)
		{
			if (typeof(TItem) == typeof(SKDZone))
			{
				var elementZone = (IElementZone)element;
				elementZone.BackgroundColor = GetSKDZoneColor(item as SKDZone);
				elementZone.SetZLayer(item == null ? 50 : 60);
			}
			else
				base.UpdateElementProperties<TItem>(element, item);
		}

		private Color GetSKDZoneColor(SKDZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}
	}
}