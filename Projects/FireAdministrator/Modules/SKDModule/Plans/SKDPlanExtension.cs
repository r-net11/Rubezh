using System;
using System.Collections.Generic;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using SKDModule.Plans.Designer;
using SKDModule.Plans.InstrumentAdorners;
using SKDModule.Plans.ViewModels;
using SKDModule.ViewModels;

namespace SKDModule.Plans
{
	class SKDPlanExtension : IPlanExtension<Plan>
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DoorsViewModel _doorsViewModel;
		private CommonDesignerCanvas _designerCanvas;
		private IEnumerable<IInstrument> _instruments;

		public SKDPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, DoorsViewModel doorsViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_doorsViewModel = doorsViewModel;
			_instruments = null;
		}

		public void Initialize()
		{
			using (new TimeCounter("DevicePictureCache.LoadSKDCache: {0}"))
				PictureCacheSource.SKDDevicePicture.LoadCache();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 1; }
		}
		public string Title
		{
			get { return "СКД Устройства"; }
		}

		public IEnumerable<IInstrument> Instruments
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
							Adorner = new SKDZoneRectangleAdorner(_designerCanvas, _zonesViewModel),
							Index = 300,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="СКД Зона",
							Adorner = new SKDZonePolygonAdorner(_designerCanvas, _zonesViewModel),
							Index = 301,
							Autostart = true
						},
					};
				return _instruments;
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementSKDDevice)
			{
				var elementSKDDevice = element as ElementSKDDevice;
				Helper.SetSKDDevice(elementSKDDevice);
				plan.ElementSKDDevices.Add(elementSKDDevice);
				return true;
			}
			else if (element is ElementDoor)
			{
				var elementDoor = element as ElementDoor;
				Helper.SetDoor(elementDoor);
				plan.ElementDoors.Add(elementDoor);
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
				Designer.Helper.SetSKDZone((IElementZone)element);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementSKDDevice)
			{
				var elementSKDDevice = (ElementSKDDevice)element;
				plan.ElementSKDDevices.Remove(elementSKDDevice);
				return true;
			}
			else if (element is ElementDoor)
			{
				var elementDoor = (ElementDoor)element;
				plan.ElementDoors.Remove(elementDoor);
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
				return true;
			}
			return false;
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleSKDZone || designerItem.Element is ElementPolygonSKDZone)
			{
				designerItem.ItemPropertyChanged += SKDZonePropertyChanged;
				OnSKDZonePropertyChanged(designerItem);
				designerItem.Group = "SKDZone";
				designerItem.IconSource = "/Controls;component/Images/zone.png";
				designerItem.UpdateProperties += UpdateDesignerItemSKDZone;
				UpdateDesignerItemSKDZone(designerItem);
			}
			else if (designerItem.Element is ElementSKDDevice)
			{
				designerItem.ItemPropertyChanged += SKDDevicePropertyChanged;
				OnSKDDevicePropertyChanged(designerItem);
				designerItem.Group = "SKD";
				designerItem.UpdateProperties += UpdateDesignerItemSKDDevice;
				UpdateDesignerItemSKDDevice(designerItem);
			}
			else if (designerItem.Element is ElementDoor)
			{
				designerItem.ItemPropertyChanged += DoorPropertyChanged;
				OnDoorPropertyChanged(designerItem);
				designerItem.Group = "Doors";
				designerItem.IconSource = "/Controls;component/Images/Door.png";
				designerItem.UpdateProperties += UpdateDesignerItemDoor;
				UpdateDesignerItemDoor(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
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

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("SKD", "СКД Устройства", 5);
			LayerGroupService.Instance.RegisterGroup("SKDZone", "СКД Зоны", 6);
			LayerGroupService.Instance.RegisterGroup("Doors", "Двери", 7);
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		#endregion

		private void UpdateDesignerItemSKDDevice(CommonDesignerItem designerItem)
		{
			ElementSKDDevice elementDevice = designerItem.Element as ElementSKDDevice;
			SKDDevice device = Designer.Helper.GetSKDDevice(elementDevice);
			Designer.Helper.SetSKDDevice(elementDevice, device);
			designerItem.Title = Helper.GetSKDDeviceTitle(elementDevice);
			designerItem.IconSource = Helper.GetSKDDeviceImageSource(elementDevice);
		}
		private void UpdateDesignerItemSKDZone(CommonDesignerItem designerItem)
		{
			IElementZone elementZone = designerItem.Element as IElementZone;
			var zone = Designer.Helper.GetSKDZone(elementZone);
			Designer.Helper.SetSKDZone(elementZone, zone);
			designerItem.Title = Designer.Helper.GetSKDZoneTitle(zone);
			elementZone.BackgroundColor = Designer.Helper.GetSKDZoneColor(zone);
			elementZone.SetZLayer(zone == null ? 50 : 60);
		}
		private void UpdateDesignerItemDoor(CommonDesignerItem designerItem)
		{
			ElementDoor elementDoor = designerItem.Element as ElementDoor;
			Door door = Designer.Helper.GetDoor(elementDoor);
			Designer.Helper.SetDoor(elementDoor, door);
			designerItem.Title = Helper.GetDoorTitle(elementDoor);
		}

		private void SKDZonePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnSKDZonePropertyChanged(designerItem);
		}
		private void OnSKDZonePropertyChanged(DesignerItem designerItem)
		{
			var zone = Designer.Helper.GetSKDZone((IElementZone)designerItem.Element);
			if (zone != null)
				zone.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildZoneMap();
						UpdateDesignerItemSKDZone(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void SKDDevicePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnSKDDevicePropertyChanged(designerItem);
		}
		private void OnSKDDevicePropertyChanged(DesignerItem designerItem)
		{
			var device = Designer.Helper.GetSKDDevice((ElementSKDDevice)designerItem.Element);
			if (device != null)
				device.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildDeviceMap();
						UpdateDesignerItemSKDDevice(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void DoorPropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDoorPropertyChanged(designerItem);
		}
		private void OnDoorPropertyChanged(DesignerItem designerItem)
		{
			var door = Designer.Helper.GetDoor((ElementDoor)designerItem.Element);
			if (door != null)
				door.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildDeviceMap();
						UpdateDesignerItemDoor(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementSKDDevice = args.Element as ElementSKDDevice;
			if (elementSKDDevice != null)
				args.Painter = new Painter(_designerCanvas, elementSKDDevice);
			else if (args.Element is ElementDoor)
				args.Painter = new DoorPainter(_designerCanvas, (ElementDoor)args.Element);
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


		public MapSource Cahce { get; private set; }

		public void RegisterDesignerItem<TElement, TItem>(DesignerItem designerItem, string group, string iconSource)
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			designerItem.ItemPropertyChanged += DesignerItemPropertyChanged<TElement, TItem>;
			OnDesignerItemPropertyChanged<TElement, TItem>(designerItem);
			designerItem.Group = group;
			designerItem.IconSource = iconSource;
			designerItem.UpdateProperties += UpdateDesignerItemProperties<TElement, TItem>;
			UpdateDesignerItemProperties<TElement, TItem>(designerItem);
		}
		private void DesignerItemPropertyChanged<TElement, TItem>(object sender, EventArgs e)
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDesignerItemPropertyChanged<TElement, TItem>(designerItem);
		}
		private void OnDesignerItemPropertyChanged<TElement, TItem>(DesignerItem designerItem)
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			var item = GetItem<TElement, TItem>(designerItem.Element);
			if (item != null)
				item.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildZoneMap();
						UpdateDesignerItemProperties<TElement, TItem>(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}
		private void UpdateDesignerItemProperties<TElement, TItem>(CommonDesignerItem designerItem)
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			TElement element = designerItem.Element as TElement;
			var item = GetItem<TElement, TItem>(element);
			if (item != null)
			{

				// ... HELPER
				//Door door = Designer.Helper.GetDoor(elementDoor);
				//Designer.Helper.SetDoor(elementDoor, door);
				//designerItem.Title = Helper.GetDoorTitle(elementDoor);
			}
		}
		private TItem GetItem<TElement, TItem>(TElement element)
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			var uid = GetElementReference<TElement>(element);
			return Cahce.Get<TItem>(uid);
		}
		private Guid GetElementReference<TElement>(TElement element)
			where TElement : ElementBase
		{
			// switch -> return element.DeviceUID
			return Guid.NewGuid();
		}
		public interface IChangedNotification : IIdentity
		{
			event Action Changed;
		}



		public class PlanHelper<TElement, TItem>
			where TElement : ElementBase
			where TItem : IChangedNotification
		{
			private static Dictionary<Guid, SKDZone> _zoneMap;
			
			public static string GetSKDZoneTitle(IElementZone element)
			{
				SKDZone zone = GetSKDZone(element);
				return GetSKDZoneTitle(zone);
			}
			public static string GetSKDZoneTitle(SKDZone zone)
			{
				return zone == null ? "Несвязанная зона" : zone.Name;
			}
			public static SKDZone GetSKDZone(IElementZone element)
			{
				return GetSKDZone(element.ZoneUID);
			}
			public static SKDZone GetSKDZone(Guid zoneUID)
			{
				return zoneUID != Guid.Empty && _zoneMap.ContainsKey(zoneUID) ? _zoneMap[zoneUID] : null;
			}
			public static void SetSKDZone(IElementZone element)
			{
				SKDZone zone = GetSKDZone(element);
				SetSKDZone(element, zone);
			}
			public static void SetSKDZone(IElementZone element, Guid zoneUID)
			{
				SKDZone zone = GetSKDZone(zoneUID);
				SetSKDZone(element, zone);
			}
			public static void SetSKDZone(IElementZone element, SKDZone zone)
			{
				ResetSKDZone(element);
				element.ZoneUID = zone == null ? Guid.Empty : zone.UID;
				element.BackgroundColor = GetSKDZoneColor(zone);
				if (zone != null)
					zone.PlanElementUIDs.Add(element.UID);
			}
			public static void ResetSKDZone(IElementZone element)
			{
				SKDZone zone = GetSKDZone(element);
				if (zone != null)
					zone.PlanElementUIDs.Remove(element.UID);
			}
			public static Color GetSKDZoneColor(SKDZone zone)
			{
				Color color = Colors.Black;
				if (zone != null)
					color = Colors.Green;
				return color;
			}
		}
	}
}