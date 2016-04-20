using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using DeviceControls;
using DevicesModule.Plans.Designer;
using DevicesModule.Plans.InstrumentAdorners;
using DevicesModule.Plans.ViewModels;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Elements;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Services;
using FiresecAPI;

namespace DevicesModule.Plans
{
	public class PlanExtension : IPlanExtension<Plan>
	{
		private static PlanExtension _instance;
		private bool _processChanges;
		private DevicesViewModel _devicesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		private ZonesViewModel _zonesViewModel;
		private IEnumerable<IInstrument> _instruments;
		public PlanExtension(DevicesViewModel devicesViewModel, ZonesViewModel zonesViewModel)
		{
			_instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(UpdateDeviceInZones);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(UpdateDeviceInZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(UpdateDeviceInZones);
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(UpdateDeviceInZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateDeviceInZones);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateDeviceInZones);

			_devicesViewModel = devicesViewModel;
			_zonesViewModel = zonesViewModel;
			_instruments = null;
			_processChanges = true;
		}

		public void Initialize()
		{
			using (new TimeCounter("DevicePictureCache.LoadCache: {0}"))
				PictureCacheSource.DevicePicture.LoadCache();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 1; }
		}
		public string Title
		{
			get { return "AC Устройства"; }
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
							ToolTip="AC Зона",
							Adorner = new ZoneRectangleAdorner(_designerCanvas, _zonesViewModel),
							Index = 100,
							Autostart = true,
							GroupIndex = 100,
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="AC Зона",
							Adorner = new ZonePolygonAdorner(_designerCanvas, _zonesViewModel),
							Index = 101,
							Autostart = true,
							GroupIndex = 100,
						},
					};
				return _instruments;
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			IElementZone elementZone = element as IElementZone;
			if (elementZone != null)
			{
				if (elementZone is ElementRectangleZone)
					plan.ElementRectangleZones.Add((ElementRectangleZone)elementZone);
				else if (elementZone is ElementPolygonZone)
					plan.ElementPolygonZones.Add((ElementPolygonZone)elementZone);
				else
					return false;
				return true;
			}
			else if (element is ElementDevice)
			{
				ElementDevice elementDevice = element as ElementDevice;
				Designer.Helper.SetDevice(elementDevice);
				plan.ElementDevices.Add(elementDevice);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			IElementZone elementZone = element as IElementZone;
			if (elementZone != null)
			{
				if (elementZone is ElementRectangleZone)
					plan.ElementRectangleZones.Remove((ElementRectangleZone)elementZone);
				else if (elementZone is ElementPolygonZone)
					plan.ElementPolygonZones.Remove((ElementPolygonZone)elementZone);
				else
					return false;
				return true;
			}
			else
			{
				ElementDevice elementDevice = element as ElementDevice;
				if (elementDevice == null)
					return false;
				else
				{
					plan.ElementDevices.Remove(elementDevice);
					return true;
				}
			}
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleZone || designerItem.Element is ElementPolygonZone)
			{
				designerItem.ItemPropertyChanged += new EventHandler(ZonePropertyChanged);
				OnZonePropertyChanged(designerItem);
				designerItem.Group = "Zone";
				designerItem.UpdateProperties += UpdateDesignerItemZone;
				UpdateDesignerItemZone(designerItem);
			}
			else if (designerItem.Element is ElementDevice)
			{
				designerItem.ItemPropertyChanged += new EventHandler(DevicePropertyChanged);
				OnDevicePropertyChanged(designerItem);
				designerItem.Group = "Devices";
				designerItem.UpdateProperties += UpdateDesignerItemDevice;
				UpdateDesignerItemDevice(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleZones)
				yield return element;
			foreach (var element in plan.ElementPolygonZones)
				yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("Devices", "АС Устройства", 0);
			LayerGroupService.Instance.RegisterGroup("Zone", "АС Зоны", 1);
			UpdateDeviceInZones(_designerCanvas.Items.Select(item => item.Element).ToList());
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("Device.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		public IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			if (GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.NotBoundedElements))
				FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
				{
					errors.AddRange(BasePlanExtension.FindUnbindedErrors<ElementDevice, ShowDeviceEvent, Guid>(plan.ElementDevices, plan.UID, "Несвязанное устройство", "/Controls;component/GKIcons/RM_1.png", Guid.Empty));
					errors.AddRange(BasePlanExtension.FindUnbindedErrors<ElementRectangleZone, ShowZoneEvent, Guid>(plan.ElementRectangleZones, plan.UID, "Несвязанная зона", "/Controls;component/Images/Zone.png", Guid.Empty));
					errors.AddRange(BasePlanExtension.FindUnbindedErrors<ElementPolygonZone, ShowZoneEvent, Guid>(plan.ElementPolygonZones, plan.UID, "Несвязанная зона", "/Controls;component/Images/Zone.png", Guid.Empty));
				});
			return errors;
		}

		#endregion

		private void UpdateDesignerItemDevice(CommonDesignerItem designerItem)
		{
			ElementDevice elementDevice = designerItem.Element as ElementDevice;
			Device device = Designer.Helper.GetDevice(elementDevice);
			Designer.Helper.SetDevice(elementDevice, device);
			designerItem.Title = Designer.Helper.GetDeviceTitle((ElementDevice)designerItem.Element);
		}
		private void UpdateDesignerItemZone(CommonDesignerItem designerItem)
		{
			IElementZone elementZone = designerItem.Element as IElementZone;
			Zone zone = Designer.Helper.GetZone(elementZone);
			Designer.Helper.SetZone(elementZone, zone);
			designerItem.Title = Designer.Helper.GetZoneTitle(zone);
			elementZone.BackgroundColor = Designer.Helper.GetZoneColor(zone);
			if (zone == null)
				elementZone.SetZLayer(20);
			else
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						elementZone.SetZLayer(30);
						break;
					case ZoneType.Guard:
						elementZone.SetZLayer(40);
						break;
				}
		}

		private void ZonePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnZonePropertyChanged(designerItem);
		}
		private void OnZonePropertyChanged(DesignerItem designerItem)
		{
			Zone zone = Designer.Helper.GetZone((IElementZone)designerItem.Element);
			if (zone != null)
				zone.ColorTypeChanged += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildZoneMap();
						UpdateDesignerItemZone(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void DevicePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDevicePropertyChanged(designerItem);
		}
		private void OnDevicePropertyChanged(DesignerItem designerItem)
		{
			Device device = Designer.Helper.GetDevice((ElementDevice)designerItem.Element);
			if (device != null)
				device.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildDeviceMap();
						UpdateDesignerItemDevice(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementDevice = args.Element as ElementDevice;
			if (elementDevice != null)
				args.Painter = new Painter(_designerCanvas, elementDevice);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementDevice element = e.Element as ElementDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleZone || e.Element is ElementPolygonZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element, _zonesViewModel);
		}

		public void UpdateDeviceInZones(List<ElementBase> items)
		{
			bool deviceInZonesChanged = false;
			if (_processChanges && !_designerCanvas.IsLocked)
				foreach (var item in items)
					if (item is ElementDevice || item is IElementZone)
					{
						deviceInZonesChanged = true;
						break;
					}
			if (deviceInZonesChanged)
			{
				var deviceInZones = new Dictionary<Device, Guid>();
				var handledDevices = new List<Device>();
				using (new WaitWrapper())
				using (new TimeCounter("\tUpdateDeviceInZones: {0}"))
				{
					Dictionary<Geometry, IElementZone> geometries = new Dictionary<Geometry, IElementZone>();
					foreach (var designerItem in _designerCanvas.Items)
					{
						var elementZone = designerItem.Element as IElementZone;
						if (elementZone != null && elementZone.ZoneUID != Guid.Empty)
							geometries.Add(((IGeometryPainter)designerItem.Painter).Geometry, elementZone);
					}
					foreach (var designerItem in _designerCanvas.Items)
					{
						ElementDevice elementDevice = designerItem.Element as ElementDevice;
						if (elementDevice != null)
						{
							var device = Designer.Helper.GetDevice(elementDevice);
							if (device == null || device.Driver == null || !device.Driver.IsZoneDevice || handledDevices.Contains(device))
								continue;
							var point = new Point(elementDevice.Left, elementDevice.Top);
							var zones = new List<IElementZone>();
							foreach (var pair in geometries)
								if (pair.Key.Bounds.Contains(point) && pair.Key.FillContains(point))
									zones.Add(pair.Value);
							if (device.ZoneUID != Guid.Empty)
							{
								var isInZone = zones.Any(x => x.ZoneUID == device.ZoneUID);
								if (!isInZone)
								{
									if (!deviceInZones.ContainsKey(device))
										deviceInZones.Add(device, GetTopZoneUID(zones));
								}
								else
								{
									handledDevices.Add(device);
									if (deviceInZones.ContainsKey(device))
										deviceInZones.Remove(device);
								}
							}
							else if (zones.Count > 0)
							{
								var zone = Helper.GetZone(GetTopZoneUID(zones));
								if (zone != null)
								{
									FiresecManager.FiresecConfiguration.AddDeviceToZone(device, zone);
									handledDevices.Add(device);
								}
							}
						}
					}
				}
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
		private Guid GetTopZoneUID(List<IElementZone> zones)
		{
			return zones.OrderByDescending(item => item.ZIndex).Select(item => item.ZoneUID).FirstOrDefault();
		}

		public static void Invalidate(List<Guid> planUIDs)
		{
			if (_instance != null && _instance._designerCanvas != null)
				foreach (var designerItem in _instance._designerCanvas.Items)
					if (planUIDs.Contains(designerItem.Element.UID))
					{
						designerItem.UpdateElementProperties();
						_instance.OnDevicePropertyChanged(designerItem);
						designerItem.Painter.Invalidate();
					}
			if (_instance._designerCanvas != null)
				_instance._designerCanvas.Refresh();
		}
	}
}
