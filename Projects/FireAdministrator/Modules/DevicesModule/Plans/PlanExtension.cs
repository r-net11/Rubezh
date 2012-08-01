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

namespace DevicesModule.Plans
{
	class PlanExtension : IPlanExtension<Plan>
	{
		private DevicesViewModel _devicesViewModel;
		public PlanExtension()
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);
			_devicesViewModel = new DevicesViewModel();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 0; }
		}
		public string Alias
		{
			get { return "Devices"; }
		}
		public string Title
		{
			get { return "Устройства"; }
		}

		public object TabPage
		{
			get { return _devicesViewModel; }
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			ElementDevice elementDevice = element as ElementDevice;
			if (elementDevice == null)
				return false;
			else
			{
				//Helper.SetDevice(elementDevice);
				plan.ElementDevices.Add(elementDevice);
				return true;
			}
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
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

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementDevice)
			{
				designerItem.Group = Alias;
				designerItem.UpdateProperties += UpdateDesignerItem;
				UpdateDesignerItem(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			return plan.ElementDevices;
		}

		#endregion

		private void UpdateDesignerItem(DesignerItem designerItem)
		{
			//designerItem.Title = Helper.GetDeviceTitle((ElementDevice)designerItem.Element);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.Element is ElementDevice)
				args.Painter = new Painter();
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			//ElementDevice element = e.Element as ElementDevice;
			//if (element != null)
			//    e.PropertyViewModel = new XDevicePropertiesViewModel(_xdevicesViewModel, element);
		}
	}
}
