using System.Collections.Generic;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.InstrumentAdorners;
using GKModule.Plans.ViewModels;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using XFiresecAPI;

namespace GKModule.Plans
{
	class GKPlanExtension : IPlanExtension<Plan>
	{
		private XDevicesViewModel _devicesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		public GKPlanExtension(GKModule.ViewModels.DevicesViewModel devicesViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(x => { UpdateXDeviceInXZones(); });
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(x => { UpdateXDeviceInXZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(x => { UpdateXDeviceInXZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(x => { UpdateXDeviceInXZones(); });

			_devicesViewModel = new XDevicesViewModel(devicesViewModel);
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 1; }
		}
		public string Title
		{
			get { return "ГК"; }
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
							ToolTip="ГК Зона",
							Adorner = new XZoneRectangleAdorner(_designerCanvas),
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="ГК Зона",
							Adorner = new XZonePolygonAdorner(_designerCanvas),
							Autostart = true
						},
					};
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			IElementZone elementXZone = element as IElementZone;
			if (elementXZone != null)
			{
				if (elementXZone is ElementRectangleXZone)
					plan.ElementRectangleXZones.Add((ElementRectangleXZone)elementXZone);
				else if (elementXZone is ElementPolygonXZone)
					plan.ElementPolygonXZones.Add((ElementPolygonXZone)elementXZone);
				else
					return false;
				Designer.Helper.SetXZone(elementXZone);
				return true;
			}
			else if (element is ElementXDevice)
			{
				ElementXDevice elementXDevice = element as ElementXDevice;
				Helper.SetXDevice(elementXDevice);
				plan.ElementXDevices.Add(elementXDevice);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			IElementZone elementXZone = element as IElementZone;
			if (elementXZone != null)
			{
				if (elementXZone is ElementRectangleXZone)
					plan.ElementRectangleXZones.Remove((ElementRectangleXZone)elementXZone);
				else if (elementXZone is ElementPolygonXZone)
					plan.ElementPolygonXZones.Remove((ElementPolygonXZone)elementXZone);
				else
					return false;
				return true;
			}
			else
			{
				ElementXDevice elementXDevice = element as ElementXDevice;
				if (elementXDevice == null)
					return false;
				else
				{
					plan.ElementXDevices.Remove(elementXDevice);
					return true;
				}
			}
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleXZone || designerItem.Element is ElementPolygonXZone)
			{
				designerItem.Group = "XZone";
				designerItem.UpdateProperties += UpdateDesignerItemXZone;
				UpdateDesignerItemXZone(designerItem);
			}
			else if (designerItem.Element is ElementXDevice)
			{
				designerItem.Group = "GK";
				designerItem.UpdateProperties += UpdateDesignerItemXDevice;
				UpdateDesignerItemXDevice(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementPolygonXZones == null)
				plan.ElementPolygonXZones = new List<ElementPolygonXZone>();
			if (plan.ElementRectangleXZones == null)
				plan.ElementRectangleXZones = new List<ElementRectangleXZone>();
			foreach (var element in plan.ElementXDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleXZones)
				yield return element;
			foreach (var element in plan.ElementPolygonXZones)
				yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("GK", "ГК", 1);
			LayerGroupService.Instance.RegisterGroup("XZone", "ГК Зоны", 2);
		}

		#endregion

		private void UpdateDesignerItemXDevice(DesignerItem designerItem)
		{
			designerItem.Title = Helper.GetXDeviceTitle((ElementXDevice)designerItem.Element);
		}
		private void UpdateDesignerItemXZone(DesignerItem designerItem)
		{
			designerItem.Title = Designer.Helper.GetXZoneTitle((IElementZone)designerItem.Element);
			IElementZone elementZone = designerItem.Element as IElementZone;
			XZone zone = Designer.Helper.GetXZone(elementZone);
			elementZone.ZLayerIndex = zone == null ? 5 : 6;
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.Element is ElementXDevice)
				args.Painter = new Painter();
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementXDevice element = e.Element as ElementXDevice;
			if (element != null)
				e.PropertyViewModel = new XDevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleXZone || e.Element is ElementPolygonXZone)
				e.PropertyViewModel = new XZonePropertiesViewModel((IElementZone)e.Element);
		}

		public void UpdateXDeviceInXZones()
		{
			///
		}
	}
}