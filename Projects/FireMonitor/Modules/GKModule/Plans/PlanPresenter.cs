using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;

namespace GKModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan>
	{
		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowXDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Subscribe(OnShowXZoneOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			var monitor = new PlanMonitor(plan, callBack);
			_monitors.Add(plan, monitor);
		}

		public int GetState(Plan plan)
		{
			return (int)(_monitors.ContainsKey(plan) ? _monitors[plan].GetState() : StateType.No);
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementXDevices.Where(x => x.XDeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			XDevicePainter devicePainter = presenterItem.Painter as XDevicePainter;
			if (devicePainter != null)
				devicePainter.Bind(presenterItem);
			else if (presenterItem.Element is IElementZone)
				presenterItem.OverridePainter(new XZonePainter(presenterItem));
		}

		#endregion

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			ElementXDevice elementXDevice = args.Element as ElementXDevice;
			if (elementXDevice != null)
				args.Painter = new XDevicePainter(elementXDevice);
		}

		private void OnShowXDeviceOnPlan(XDevice xdevice)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementXDevices)
					if (element.XDeviceUID == xdevice.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowXZoneOnPlan(XZone xzone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementRectangleXZones)
					if (element.ZoneUID == xzone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementPolygonXZones)
					if (element.ZoneUID == xzone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}
