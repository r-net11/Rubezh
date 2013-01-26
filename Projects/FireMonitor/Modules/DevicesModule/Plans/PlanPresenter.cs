using System;
using System.Collections.Generic;
using System.Linq;
using DevicesModule.Plans.Designer;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using Common;

namespace DevicesModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan>
	{
		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Subscribe(OnShowZoneOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			Helper.BuildMap();
			var monitor = new PlanMonitor(plan, callBack);
			_monitors.Add(plan, monitor);
		}
		public int GetState(Plan plan)
		{
			return (int)(_monitors.ContainsKey(plan) ? _monitors[plan].GetState() : StateType.No);
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementDevices.Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementDevice)
				presenterItem.OverridePainter(new DevicePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonZone || presenterItem.Element is ElementRectangleZone)
				presenterItem.OverridePainter(new ZonePainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("Device.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		#endregion

		public void Clear()
		{
			_monitors.Clear();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowDeviceOnPlan(Guid deviceUID)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementDevices)
					if (element.DeviceUID == deviceUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowZoneOnPlan(Guid zoneUID)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementRectangleZones)
					if (element.ZoneUID == zoneUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementPolygonZones)
					if (element.ZoneUID == zoneUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}
