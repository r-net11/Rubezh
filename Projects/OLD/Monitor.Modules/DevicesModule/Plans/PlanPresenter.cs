using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using DevicesModule.Plans.Designer;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Plans;
using Infrastructure.Plans.Elements;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Presenter;

namespace DevicesModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
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
			if (_monitors.ContainsKey(plan))
				_monitors[plan].AddCallBack(callBack);
			else
				_monitors.Add(plan, new PlanMonitor(plan, callBack));
		}
		public XStateClass GetState(Plan plan)
		{
			return _monitors.ContainsKey(plan) ? _monitors[plan].GetState() : XStateClass.No;
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementDevices.Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
				yield return element;
			foreach (var element in plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
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

		public void Initialize()
		{
			_monitors.Clear();
			using (new TimeCounter("DevicePictureCache.LoadCache: {0}"))
				PictureCacheSource.DevicePicture.LoadCache();
			using (new TimeCounter("DevicePictureCache.LoadDynamicCache: {0}"))
				PictureCacheSource.DevicePicture.LoadDynamicCache();
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
