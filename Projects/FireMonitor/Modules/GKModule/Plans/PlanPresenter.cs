using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Events;
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
			Cache.Add<GKZone>(() => GKManager.Zones);
			Cache.Add<GKGuardZone>(() => GKManager.DeviceConfiguration.GuardZones);
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKDirection>(() => GKManager.Directions);
			Cache.Add<GKDoor>(() => GKManager.Doors);

			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowXDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Subscribe(OnShowXZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXGuardZoneOnPlanEvent>().Subscribe(OnShowXGuardZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Subscribe(OnShowXDirectionOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKDoorOnPlanEvent>().Subscribe(OnShowGKDoorOnPlan);
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
			foreach (var element in plan.ElementGKDevices.Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleGKZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementPolygonGKZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementRectangleGKGuardZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementPolygonGKGuardZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHidden))
				yield return element;
			foreach (var element in plan.ElementRectangleGKDirections.Where(x => x.DirectionUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementPolygonGKDirections.Where(x => x.DirectionUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementGKDoors.Where(x => x.DoorUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementGKDevice)
				presenterItem.OverridePainter(new XDevicePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKZone || presenterItem.Element is ElementRectangleGKZone)
				presenterItem.OverridePainter(new XZonePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKGuardZone || presenterItem.Element is ElementRectangleGKGuardZone)
				presenterItem.OverridePainter(new XGuardZonePainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleGKDirection || presenterItem.Element is ElementPolygonGKDirection)
				presenterItem.OverridePainter(new XDirectionPainter(presenterItem));
			else if (presenterItem.Element is ElementGKDoor)
				presenterItem.OverridePainter(new GKDoorPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("GKDevice.ExtensionAttached.BuildMap: {0}"))
				Cache.BuildAllSafe();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
			using (new TimeCounter("DevicePictureCache.LoadXCache: {0}"))
				PictureCacheSource.GKDevicePicture.LoadCache();
			using (new TimeCounter("DevicePictureCache.LoadXDynamicCache: {0}"))
				PictureCacheSource.GKDevicePicture.LoadDynamicCache();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowXDeviceOnPlan(GKDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDevices)
					if (element.DeviceUID == device.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowXZoneOnPlan(GKZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowXGuardZoneOnPlan(GKGuardZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKGuardZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKGuardZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowXDirectionOnPlan(GKDirection direction)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKDirections)
					if (element.DirectionUID == direction.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKDirections)
					if (element.DirectionUID == direction.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowGKDoorOnPlan(GKDoor door)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDoors)
					if (element.DoorUID == door.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}