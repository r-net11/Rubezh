using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
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
			ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Subscribe(OnShowXDirectionOnPlan);
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
			return (int)(_monitors.ContainsKey(plan) ? _monitors[plan].GetState() : XStateClass.No);
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementXDevices.Where(x => x.XDeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementRectangleXDirections.Where(x => x.DirectionUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementPolygonXDirections.Where(x => x.DirectionUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementXDevice)
				presenterItem.OverridePainter(new XDevicePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonXZone || presenterItem.Element is ElementRectangleXZone)
				presenterItem.OverridePainter(new XZonePainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleXDirection || presenterItem.Element is ElementPolygonXDirection)
				presenterItem.OverridePainter(new XDirectionPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
			using (new TimeCounter("DevicePictureCache.LoadXCache: {0}"))
				DevicePictureCache.LoadXCache();
			using (new TimeCounter("DevicePictureCache.LoadXDynamicCache: {0}"))
				DevicePictureCache.LoadXDynamicCache();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
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
			{
				foreach (var element in plan.ElementRectangleXZones)
					if (element.ZoneUID == xzone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonXZones)
					if (element.ZoneUID == xzone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowXDirectionOnPlan(XDirection xdirection)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleXDirections)
					if (element.DirectionUID == xdirection.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonXDirections)
					if (element.DirectionUID == xdirection.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
	}
}