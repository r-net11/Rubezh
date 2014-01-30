using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using SKDModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;

namespace SKDModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan>
	{
		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowSKDDeviceOnPlan);
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
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementSKDDevice)
				presenterItem.OverridePainter(new SKDDevicePainter(presenterItem));
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

		private void OnShowSKDDeviceOnPlan(XDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementSKDDevices)
					if (element.DeviceUID == device.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}