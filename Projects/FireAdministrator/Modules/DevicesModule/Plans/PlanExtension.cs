using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrustructure.Plans;
using Infrastructure;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Designer;
using DeviceControls;
using Infrustructure.Plans.Elements;
using DevicesModule.Plans.ViewModels;
using DevicesModule.Plans.InstrumentAdorners;
using Infrustructure.Plans.Services;
using DevicesModule.Plans.Events;
using Infrastructure.Common.Windows;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace DevicesModule.Plans
{
	public class PlanExtension : IPlanExtension<Plan>
	{
		private DevicesViewModel _devicesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		public PlanExtension()
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(UpdateDevice);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(UpdateDevice);

			_devicesViewModel = new DevicesViewModel();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 0; }
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
				return new List<IInstrument>()
					{
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZoneRectangle.png",
							ToolTip="Зона",
							Adorner = new ZoneRectangleAdorner(_designerCanvas),
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="Зона",
							Adorner = new ZonePolygonAdorner(_designerCanvas),
							Autostart = true
						},
					};
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			IElementZone elementZone = element as IElementZone;
			if (elementZone != null)
			{
				Designer.Helper.SetZone(elementZone);
				if (elementZone is ElementRectangleZone)
					plan.ElementRectangleZones.Add((ElementRectangleZone)elementZone);
				else if (elementZone is ElementPolygonZone)
					plan.ElementPolygonZones.Add((ElementPolygonZone)elementZone);
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
			if (designerItem.Element is IElementZone)
			{
				designerItem.Group = "Zone";
				designerItem.UpdateProperties += new Action<DesignerItem>(UpdateDesignerItemZone);
				UpdateDesignerItemZone(designerItem);
			}
			if (designerItem.Element is ElementDevice)
			{
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
			LayerGroupService.Instance.RegisterGroup("Devices", "Устройства", 0);
			LayerGroupService.Instance.RegisterGroup("Zone", "Зоны", 1);
			UpdateDeviceInZones();
		}

		#endregion

		private void UpdateDesignerItemDevice(DesignerItem designerItem)
		{
			designerItem.Title = Designer.Helper.GetDeviceTitle((ElementDevice)designerItem.Element);
		}
		private void UpdateDesignerItemZone(DesignerItem designerItem)
		{
			designerItem.Title = Designer.Helper.GetZoneTitle((IElementZone)designerItem.Element);
			IElementZone elementZone = designerItem.Element as IElementZone;
			Zone zone = Designer.Helper.GetZone(elementZone);
			if (zone == null)
				elementZone.ZLayerIndex = 2;
			else
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						elementZone.ZLayerIndex = 3;
						break;
					case ZoneType.Guard:
						elementZone.ZLayerIndex = 4;
						break;
				}
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.Element is ElementDevice)
				args.Painter = new Painter();
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementDevice element = e.Element as ElementDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is IElementZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element);
		}

		private void UpdateDevice(List<ElementBase> elements)
		{
			foreach (var element in elements.OfType<ElementDevice>())
				Designer.Helper.ResetDevice(element);
		}
		public void UpdateDeviceInZones()
		{
			var deviceInZones = new Dictionary<Device, int?>();
			foreach (var designerItem in _designerCanvas.Items)
			{
				ElementDevice elementDevice = designerItem.Element as ElementDevice;
				if (elementDevice != null)
				{
					var designerItemCenterX = Canvas.GetLeft(designerItem) + designerItem.Width / 2;
					var designerItemCenterY = Canvas.GetTop(designerItem) + designerItem.Height / 2;
					var device = Designer.Helper.GetDevice(elementDevice);
					if (device == null || device.Driver == null || !device.Driver.IsZoneDevice)
						continue;
					var zones = new List<int>();
					foreach (var elementPolygonZoneItem in _designerCanvas.Items)
					{
						var point = new Point((int)(designerItemCenterX - Canvas.GetLeft(elementPolygonZoneItem)), (int)(designerItemCenterY - Canvas.GetTop(elementPolygonZoneItem)));
						ElementPolygonZone elementPolygonZone = elementPolygonZoneItem.Element as ElementPolygonZone;
						if (elementPolygonZone != null)
						{
							bool isInPolygon = Designer.Helper.IsPointInPolygon(point, elementPolygonZoneItem.Content as Polygon);
							if (isInPolygon && elementPolygonZone.ZoneNo.HasValue)
								zones.Add(elementPolygonZone.ZoneNo.Value);
						}
						ElementRectangleZone elementRectangleZone = elementPolygonZoneItem.Element as ElementRectangleZone;
						if (elementRectangleZone != null)
						{
							bool isInRectangle = ((point.X > 0) && (point.X < elementRectangleZone.Width) && (point.Y > 0) && (point.Y < elementRectangleZone.Height));
							if (isInRectangle && elementRectangleZone.ZoneNo.HasValue)
								zones.Add(elementRectangleZone.ZoneNo.Value);
						}
					}

					if (device.ZoneNo.HasValue)
					{
						var isInZone = zones.Any(x => x == device.ZoneNo.Value);
						if (!isInZone)
						{
							if (!deviceInZones.ContainsKey(device))
								deviceInZones.Add(device, zones.Count > 0 ? (int?)zones[0] : null);
							else if (zones.Count > 0)
								deviceInZones[device] = zones[0];
						}
					}
					else if (zones.Count > 0)
					{
						device.ZoneNo = zones[0];
						ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
					}
				}
			}
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
			}
		}

		public void UpdateDevices()
		{
			_devicesViewModel.Update();
		}
		public void RemoveDevice(Device device)
		{
			_designerCanvas.Remove(device.PlanElementUIDs);
		}
	}
}
