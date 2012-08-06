using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans;
using Infrustructure.Plans.Services;
using GKModule.Plans.ViewModels;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using Infrustructure.Plans.Designer;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrustructure.Plans.Events;

namespace GKModule.Plans
{
	class GKPlanExtension : IPlanExtension<Plan>
	{
		private XDevicesViewModel _xdevicesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		public GKPlanExtension()
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);
			_xdevicesViewModel = new XDevicesViewModel();
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
			get { return _xdevicesViewModel; }
		}

		public IEnumerable<IInstrument> Instruments
		{
			get { return null; }
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			ElementXDevice elementXDevice = element as ElementXDevice;
			if (elementXDevice == null)
				return false;
			else
			{
				Helper.SetXDevice(elementXDevice);
				plan.ElementXDevices.Add(elementXDevice);
				return true;
			}
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
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

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementXDevice)
			{
				designerItem.Group = "GK";
				designerItem.UpdateProperties += UpdateDesignerItem;
				UpdateDesignerItem(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			return plan.ElementXDevices;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("GK", "ГК", 1);
		}

		#endregion

		private void UpdateDesignerItem(DesignerItem designerItem)
		{
			designerItem.Title = Helper.GetXDeviceTitle((ElementXDevice)designerItem.Element);
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
				e.PropertyViewModel = new XDevicePropertiesViewModel(_xdevicesViewModel, element);
		}
	}
}
