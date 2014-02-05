using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Services;
using SKDModule.Plans.Designer;
using SKDModule.Plans.InstrumentAdorners;
using SKDModule.Plans.ViewModels;
using SKDModule.ViewModels;

namespace SKDModule.Plans
{
	class SKDPlanExtension : IPlanExtension<Plan>
	{
		private bool _processChanges;
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		private IEnumerable<IInstrument> _instruments;

		public SKDPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(UpdateSKDDeviceInSKDZones);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(UpdateSKDDeviceInSKDZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(UpdateSKDDeviceInSKDZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(UpdateSKDDeviceInSKDZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateSKDDeviceInSKDZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateSKDDeviceInSKDZones);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_instruments = null;
			_processChanges = true;
		}

		public void Initialize()
		{
			using (new TimeCounter("DevicePictureCache.LoadSKDCache: {0}"))
				DevicePictureCache.LoadSKDCache();
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

		public object TabPage
		{
			get { return _devicesViewModel; }
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
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementPolygonSKDZones == null)
				plan.ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
			if (plan.ElementRectangleSKDZones == null)
				plan.ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
			foreach (var element in plan.ElementSKDDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleSKDZones)
				yield return element;
			foreach (var element in plan.ElementPolygonSKDZones)
				yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("SKD", "СКД Устройства", 4);
			LayerGroupService.Instance.RegisterGroup("SKDZone", "СКД Зоны", 5);
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

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementSKDDevice = args.Element as ElementSKDDevice;
			if (elementSKDDevice != null)
				args.Painter = new Painter(elementSKDDevice);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementSKDDevice element = e.Element as ElementSKDDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleSKDZone || e.Element is ElementPolygonSKDZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
		}

		public void UpdateSKDDeviceInSKDZones(List<ElementBase> items)
		{
			if (IsDeviceInZonesChanged(items))
			{
				var deviceInZones = new Dictionary<SKDDevice, Guid>();
				var handledSKDDevices = new List<SKDDevice>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateXDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in _designerCanvas.Items)
					{
						var elementSKDDevice = designerItem.Element as ElementSKDDevice;
						if (elementSKDDevice != null)
						{
							var skdDevice = Designer.Helper.GetSKDDevice(elementSKDDevice);
							if (skdDevice == null || skdDevice.Driver == null || handledSKDDevices.Contains(skdDevice))
								continue;
							var point = new Point(elementSKDDevice.Left, elementSKDDevice.Top);
							var zones = new List<IElementZone>();
							foreach (var pair in geometries)
								if (pair.Key.Bounds.Contains(point) && pair.Key.FillContains(point))
									zones.Add(pair.Value);

							if (skdDevice.ZoneUID == Guid.Empty)
							{
								if (zones.Count > 0)
								{
									var zone = Helper.GetSKDZone(GetTopZoneUID(zones));
									if (zone != null)
									{
										SKDManager.AddDeviceToZone(skdDevice, zone);
										handledSKDDevices.Add(skdDevice);
									}
								}
							}
							else
							{
								var isInZone = zones.Any(x => x.ZoneUID == skdDevice.ZoneUID);
								if (!isInZone)
								{
									if (!deviceInZones.ContainsKey(skdDevice))
										deviceInZones.Add(skdDevice, GetTopZoneUID(zones));
								}
								else
								{
									handledSKDDevices.Add(skdDevice);
									if (deviceInZones.ContainsKey(skdDevice))
										deviceInZones.Remove(skdDevice);
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
			if (_processChanges && !_designerCanvas.IsLocked)
				foreach (var item in items)
					if (item is ElementSKDDevice || item is IElementZone)
						return true;
			return false;
		}
		private Dictionary<Geometry, IElementZone> GetZoneGeometryMap()
		{
			var geometries = new Dictionary<Geometry, IElementZone>();
			foreach (var designerItem in _designerCanvas.Items)
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
		private void ShowDeviceInZoneChanged(Dictionary<SKDDevice, Guid> deviceInZones)
		{
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
				if (!result)
				{
					_processChanges = false;
					_designerCanvas.RevertLastAction();
					_processChanges = true;
				}
			}
		}
	}
}