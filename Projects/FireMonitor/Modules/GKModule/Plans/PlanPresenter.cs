using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using RubezhAPI;
using Infrustructure.Plans.Interfaces;

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
			Cache.Add<GKGuardZone>(() => GKManager.GuardZones);
			Cache.Add<GKSKDZone>(() => GKManager.SKDZones);
			Cache.Add<GKDevice>(() => GKManager.Devices);
			Cache.Add<GKDelay>(() => GKManager.Delays);
			Cache.Add<GKDirection>(() => GKManager.Directions);
			Cache.Add<GKMPT>(() => GKManager.MPTs);
			Cache.Add<GKDoor>(() => GKManager.Doors);

			ServiceFactory.Events.GetEvent<ShowGKDeviceOnPlanEvent>().Subscribe(OnShowGKDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKZoneOnPlanEvent>().Subscribe(OnShowGKZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKGuardZoneOnPlanEvent>().Subscribe(OnShowGKGuardZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneOnPlanEvent>().Subscribe(OnShowGKSKDZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKDelayOnPlanEvent>().Subscribe(OnShowGKDelayOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKDirectionOnPlanEvent>().Subscribe(OnShowGKDirectionOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKPumpStationOnPlanEvent>().Subscribe(OnShowGKPumpStationOnPlan);
			ServiceFactory.Events.GetEvent<ShowGKMPTOnPlanEvent>().Subscribe(OnShowGKMPTOnPlan);
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

		public NamedStateClass GetNamedStateClass(Plan plan)
		{
			return _monitors.ContainsKey(plan) ? _monitors[plan].GetNamedStateClass() : new NamedStateClass();
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			return new ElementBase[0]
				.Concat(plan.ElementGKDevices.Where(x => x.DeviceUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKGuardZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKGuardZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKSKDZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKSKDZones.Where(x => x.ZoneUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKDelays.Where(x => x.DelayUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKDelays.Where(x => x.DelayUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKPumpStations.Where(x => x.ItemUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKPumpStations.Where(x => x.ItemUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKDirections.Where(x => x.DirectionUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKDirections.Where(x => x.DirectionUID != Guid.Empty))
				.Concat(plan.ElementRectangleGKMPTs.Where(x => x.MPTUID != Guid.Empty))
				.Concat(plan.ElementPolygonGKMPTs.Where(x => x.MPTUID != Guid.Empty))
				.Concat(plan.ElementGKDoors.Where(x => x.DoorUID != Guid.Empty));
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementGKDevice)
				presenterItem.OverridePainter(new GKDevicePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKZone || presenterItem.Element is ElementRectangleGKZone)
				presenterItem.OverridePainter(new GKZonePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKGuardZone || presenterItem.Element is ElementRectangleGKGuardZone)
				presenterItem.OverridePainter(new GKGuardZonePainter(presenterItem));
			else if (presenterItem.Element is ElementPolygonGKSKDZone || presenterItem.Element is ElementRectangleGKSKDZone)
				presenterItem.OverridePainter(new GKSKDZonePainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleGKDelay || presenterItem.Element is ElementPolygonGKDelay)
				presenterItem.OverridePainter(new GKDelayPainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleGKPumpStation || presenterItem.Element is ElementPolygonGKPumpStation)
				presenterItem.OverridePainter(new GKPumpStationPainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleGKDirection || presenterItem.Element is ElementPolygonGKDirection)
				presenterItem.OverridePainter(new GKDirectionPainter(presenterItem));
			else if (presenterItem.Element is ElementRectangleGKMPT || presenterItem.Element is ElementPolygonGKMPT)
				presenterItem.OverridePainter(new GKMPTPainter(presenterItem));
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
			using (new TimeCounter("DevicePictureCache.LoadGKCache: {0}"))
				PictureCacheSource.GKDevicePicture.LoadCache();
			using (new TimeCounter("DevicePictureCache.LoadGKDynamicCache: {0}"))
				PictureCacheSource.GKDevicePicture.LoadDynamicCache();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowGKDeviceOnPlan(GKDevice device)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDevices)
					if (element.DeviceUID == device.UID)
					{
						var eventArgs = new NavigateToPlanElementEventArgs(plan.UID, element.UID);
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(eventArgs);
						var xxx = eventArgs.WasShown;
						return;
					}
		}
		private void OnShowGKZoneOnPlan(GKZone zone)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
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
		private void OnShowGKGuardZoneOnPlan(GKGuardZone zone)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
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
		private void OnShowGKSKDZoneOnPlan(GKSKDZone zone)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKSKDZones)
					if (element.ZoneUID == zone.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowGKDelayOnPlan(GKDelay delay)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKDelays)
				{
					if (element.DelayUID == delay.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				}
				foreach (var element in plan.ElementPolygonGKDelays)
				{
					if (element.DelayUID == delay.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				}
			}
		}
		private void OnShowGKPumpStationOnPlan(GKPumpStation pumpStation)
		{
			IEnumerable<NavigateToPlanElementEventArgs> eventArgs = ClientManager.PlansConfiguration.AllPlans
				.SelectMany(plan => new IElementReference[0]
					.Concat(plan.ElementRectangleGKPumpStations)
					.Concat(plan.ElementPolygonGKPumpStations)
					.Where(element => element.ItemUID == pumpStation.UID)
					.Select(element => new NavigateToPlanElementEventArgs(plan.UID, element.UID)));
			foreach (var arg in eventArgs)
			{
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(arg);
			}
		}
		private void OnShowGKDirectionOnPlan(GKDirection direction)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
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
		private void OnShowGKMPTOnPlan(GKMPT mpt)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementRectangleGKMPTs)
					if (element.MPTUID == mpt.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
				foreach (var element in plan.ElementPolygonGKMPTs)
					if (element.MPTUID == mpt.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
			}
		}
		private void OnShowGKDoorOnPlan(GKDoor door)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementGKDoors)
					if (element.DoorUID == door.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}