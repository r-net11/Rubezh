using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using DeviceControls;
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
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Services;
using XFiresecAPI;

namespace GKModule.Plans
{
	class GKPlanExtension : IPlanExtension<Plan>
	{
		private DevicesViewModel _devicesViewModel;
		private ZonesViewModel _zonesViewModel;
		private DirectionsViewModel _directionsViewModel;
		private CommonDesignerCanvas _designerCanvas;
		private IEnumerable<IInstrument> _instruments;

		public GKPlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel, DirectionsViewModel directionsViewModel)
		{
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
			_instruments = null;
		}

		public void Initialize()
		{
			using (new TimeCounter("DevicePictureCache.LoadXCache: {0}"))
				DevicePictureCache.LoadXCache();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 1; }
		}
		public string Title
		{
			get { return "Устройства"; }
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
							ToolTip="Зона",
							Adorner = new XZoneRectangleAdorner(_designerCanvas,_zonesViewModel),
							Index = 200,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="Зона",
							Adorner = new XZonePolygonAdorner(_designerCanvas,_zonesViewModel),
							Index = 201,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/Direction.png",
							ToolTip="Направление",
							Adorner = new XDirectionRectangleAdorner(_designerCanvas, _directionsViewModel),
							Index = 202,
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/Direction.png",
							ToolTip="Направление",
							Adorner = new XDirectionPolygonAdorner(_designerCanvas, _directionsViewModel),
							Index = 203,
							Autostart = true
						},
					};
				return _instruments;
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementXDevice)
			{
				var elementXDevice = element as ElementXDevice;
				Helper.SetXDevice(elementXDevice);
				plan.ElementXDevices.Add(elementXDevice);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleXZone)
					plan.ElementRectangleXZones.Add((ElementRectangleXZone)element);
				else if (element is ElementPolygonXZone)
					plan.ElementPolygonXZones.Add((ElementPolygonXZone)element);
				else
					return false;
				Designer.Helper.SetXZone((IElementZone)element);
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
				Helper.SetXDirection((IElementDirection)element);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementXDevice)
			{
				var elementXDevice = element as ElementXDevice;
				plan.ElementXDevices.Remove(elementXDevice);
				return true;
			}
			else if (element is IElementZone)
			{
				if (element is ElementRectangleXZone)
					plan.ElementRectangleXZones.Remove((ElementRectangleXZone)element);
				else if (element is ElementPolygonXZone)
					plan.ElementPolygonXZones.Remove((ElementPolygonXZone)element);
				else
					return false;
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
				return true;
			}
			return false;
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleXZone || designerItem.Element is ElementPolygonXZone)
			{
				designerItem.ItemPropertyChanged += XZonePropertyChanged;
				OnXZonePropertyChanged(designerItem);
				designerItem.Group = "XZone";
				designerItem.IconSource = "/Controls;component/Images/zone.png";
				designerItem.UpdateProperties += UpdateDesignerItemXZone;
				UpdateDesignerItemXZone(designerItem);
			}
			else if (designerItem.Element is ElementXDevice)
			{
				designerItem.ItemPropertyChanged += XDevicePropertyChanged;
				OnXDevicePropertyChanged(designerItem);
				designerItem.Group = "GK";
				designerItem.UpdateProperties += UpdateDesignerItemXDevice;
				UpdateDesignerItemXDevice(designerItem);
			}
			else if (designerItem.Element is IElementDirection)
			{
				designerItem.ItemPropertyChanged += XDirectionPropertyChanged;
				OnXDirectionPropertyChanged(designerItem);
				designerItem.Group = "XDirection";
				designerItem.IconSource = "/Controls;component/Images/Blue_Direction.png";
				designerItem.UpdateProperties += UpdateDesignerItemXDirection;
				UpdateDesignerItemXDirection(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementPolygonXZones == null)
				plan.ElementPolygonXZones = new List<ElementPolygonXZone>();
			if (plan.ElementRectangleXZones == null)
				plan.ElementRectangleXZones = new List<ElementRectangleXZone>();
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
			foreach (var element in plan.ElementRectangleXDirections)
				yield return element;
			foreach (var element in plan.ElementPolygonXDirections)
				yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("GK", "Устройства", 1);
			LayerGroupService.Instance.RegisterGroup("XZone", "Зоны", 2);
			LayerGroupService.Instance.RegisterGroup("XDirection", "Направления", 3);
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		#endregion

		private void UpdateDesignerItemXDevice(CommonDesignerItem designerItem)
		{
			ElementXDevice elementDevice = designerItem.Element as ElementXDevice;
			XDevice device = Designer.Helper.GetXDevice(elementDevice);
			Designer.Helper.SetXDevice(elementDevice, device);
			designerItem.Title = Helper.GetXDeviceTitle(elementDevice);
			designerItem.IconSource = Helper.GetXDeviceImageSource(elementDevice);
		}
		private void UpdateDesignerItemXZone(CommonDesignerItem designerItem)
		{
			IElementZone elementZone = designerItem.Element as IElementZone;
			var xzone = Designer.Helper.GetXZone(elementZone);
			Designer.Helper.SetXZone(elementZone, xzone);
			designerItem.Title = Designer.Helper.GetXZoneTitle(xzone);
			elementZone.BackgroundColor = Designer.Helper.GetXZoneColor(xzone);
			elementZone.SetZLayer(xzone == null ? 50 : 60);
		}
		private void UpdateDesignerItemXDirection(CommonDesignerItem designerItem)
		{
			var elementXDirection = designerItem.Element as IElementDirection;
			var xdirection = Designer.Helper.GetXDirection(elementXDirection);
			Designer.Helper.SetXDirection(elementXDirection, xdirection);
			designerItem.Title = Designer.Helper.GetXDirectionTitle(xdirection);
			elementXDirection.BackgroundColor = Designer.Helper.GetXDirectionColor(xdirection);
			elementXDirection.SetZLayer(xdirection == null ? 10 : 11);
		}

		private void XZonePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnXZonePropertyChanged(designerItem);
		}
		private void OnXZonePropertyChanged(DesignerItem designerItem)
		{
			var zone = Designer.Helper.GetXZone((IElementZone)designerItem.Element);
			if (zone != null)
				zone.Changed += () =>
				{
					Helper.BuildXZoneMap();
					UpdateDesignerItemXZone(designerItem);
					designerItem.Painter.Invalidate();
					_designerCanvas.Refresh();
				};
		}

		private void XDirectionPropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnXDirectionPropertyChanged(designerItem);
		}
		private void OnXDirectionPropertyChanged(DesignerItem designerItem)
		{
			var direction = Designer.Helper.GetXDirection((IElementDirection)designerItem.Element);
			if (direction != null)
				direction.Changed += () =>
				{
					Helper.BuildXDirectionMap();
					UpdateDesignerItemXDirection(designerItem);
					designerItem.Painter.Invalidate();
					_designerCanvas.Refresh();
				};
		}

		private void XDevicePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnXDevicePropertyChanged(designerItem);
		}
		private void OnXDevicePropertyChanged(DesignerItem designerItem)
		{
			var device = Designer.Helper.GetXDevice((ElementXDevice)designerItem.Element);
			if (device != null)
				device.Changed += () =>
				{
					Helper.BuildXDeviceMap();
					UpdateDesignerItemXDevice(designerItem);
					designerItem.Painter.Invalidate();
					_designerCanvas.Refresh();
				};
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementXDevice = args.Element as ElementXDevice;
			if (elementXDevice != null)
				args.Painter = new Painter(elementXDevice);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementXDevice element = e.Element as ElementXDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleXZone || e.Element is ElementPolygonXZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
			else if (e.Element is ElementRectangleXDirection || e.Element is ElementPolygonXDirection)
				e.PropertyViewModel = new DirectionPropertiesViewModel((IElementDirection)e.Element, _directionsViewModel);
		}

		public void UpdateXDeviceInXZones(List<ElementBase> items)
		{
			if (IsDeviceInZonesChanged(items))
			{
				var deviceInZones = new Dictionary<XDevice, Guid>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateXDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = GetZoneGeometryMap();
					foreach (var designerItem in _designerCanvas.Items)
					{
						var elementXDevice = designerItem.Element as ElementXDevice;
						if (elementXDevice != null)
						{
							var xdevice = Designer.Helper.GetXDevice(elementXDevice);
							if (xdevice == null || xdevice.Driver == null)
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
										var zone = Helper.GetXZone(GetTopZoneUID(zones));
										if (zone != null)
											XManager.AddDeviceToZone(xdevice, zone);
									}
									break;
								case 1:
									var isInZone = zones.Any(x => x.ZoneUID == xdevice.ZoneUIDs[0]);
									if (!isInZone)
										deviceInZones.Add(xdevice, GetTopZoneUID(zones));
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
			foreach (var item in items)
				if (item is ElementXDevice || item is IElementZone)
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
		private void ShowDeviceInZoneChanged(Dictionary<XDevice, Guid> deviceInZones)
		{
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
				if (!result)
					_designerCanvas.RevertLastAction();
			}
		}
	}
}