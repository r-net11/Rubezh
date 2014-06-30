using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }

		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add<XZone>(() => XManager.Zones);
			Cache.Add<XGuardZone>(() => XManager.DeviceConfiguration.GuardZones);
			Cache.Add<XDevice>(() => XManager.Devices);
			Cache.Add<XDirection>(() => XManager.Directions);

			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowXDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Subscribe(OnShowXZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXGuardZoneOnPlanEvent>().Subscribe(OnShowXGuardZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Subscribe(OnShowXDirectionOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			Cache.BuildAllSafe();
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
			foreach (var element in plan.ElementXDevices.Where(x => x.XDeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementRectangleXGuardZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementPolygonXGuardZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
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
			else if (presenterItem.Element is ElementPolygonXGuardZone || presenterItem.Element is ElementRectangleXGuardZone)
				presenterItem.OverridePainter(new XGuardZonePainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleXDirection || presenterItem.Element is ElementPolygonXDirection)
				presenterItem.OverridePainter(new XDirectionPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("XDevice.ExtensionAttached.BuildMap: {0}"))
				Cache.BuildAllSafe();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
			using (new TimeCounter("DevicePictureCache.LoadXCache: {0}"))
				PictureCacheSource.XDevicePicture.LoadCache();
			using (new TimeCounter("DevicePictureCache.LoadXDynamicCache: {0}"))
				PictureCacheSource.XDevicePicture.LoadDynamicCache();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowXDeviceOnPlan(XDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementXDevices)
					if (element.XDeviceUID == device.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowXZoneOnPlan(XZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleXZones)
					if (element.ZoneUID == zone.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonXZones)
					if (element.ZoneUID == zone.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowXGuardZoneOnPlan(XGuardZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleXGuardZones)
					if (element.ZoneUID == zone.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonXGuardZones)
					if (element.ZoneUID == zone.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowXDirectionOnPlan(XDirection direction)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleXDirections)
					if (element.DirectionUID == direction.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonXDirections)
					if (element.DirectionUID == direction.BaseUID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
	}
}