using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using StrazhModule.Plans.Designer;

namespace StrazhModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }

		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add<SKDDevice>(() => SKDManager.Devices);
			Cache.Add<SKDZone>(() => SKDManager.Zones);
			Cache.Add<SKDDoor>(() => SKDManager.SKDConfiguration.Doors);

			ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Subscribe(OnShowSKDDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowSKDZoneOnPlanEvent>().Subscribe(OnShowSKDZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowSKDDoorOnPlanEvent>().Subscribe(OnShowDoorOnPlan);
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

		public NamedStateClass GetNamedStateClass(Plan plan)
		{
			return _monitors.ContainsKey(plan) ? _monitors[plan].GetNamedStateClass() : new NamedStateClass();
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementSKDDevices.Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementDoors.Where(x => x.DoorUID != Guid.Empty))
				yield return element;
			foreach (var element in plan.ElementRectangleSKDZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
				yield return element;
			foreach (var element in plan.ElementPolygonSKDZones.Where(x => x.ZoneUID != Guid.Empty && !x.IsHiddenZone))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementSKDDevice)
				presenterItem.OverridePainter(new SKDDevicePainter(presenterItem));
			else if (presenterItem.Element is ElementDoor)
				presenterItem.OverridePainter(new DoorPainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonSKDZone || presenterItem.Element is ElementRectangleSKDZone)
				presenterItem.OverridePainter(new SKDZonePainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("SKDDevice.ExtensionAttached.BuildMap: {0}"))
				Cache.BuildAllSafe();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
			using (new TimeCounter("DevicePictureCache.LoadSKDCache: {0}"))
				PictureCacheSource.SKDDevicePicture.LoadCache();
			using (new TimeCounter("DevicePictureCache.LoadSKDDynamicCache: {0}"))
				PictureCacheSource.SKDDevicePicture.LoadDynamicCache();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowSKDDeviceOnPlan(SKDDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementSKDDevices)
					if (element.DeviceUID == device.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowDoorOnPlan(SKDDoor door)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementDoors)
					if (element.DoorUID == door.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
		private void OnShowSKDZoneOnPlan(SKDZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
	}
}