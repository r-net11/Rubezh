using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using StrazhAPI.GK;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using VideoModule.Plans.Designer;

namespace VideoModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }

		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add(() => FiresecManager.SystemConfiguration.Cameras);
			Cache.BuildAll();
			ServiceFactory.Events.GetEvent<ShowCameraOnPlanEvent>().Subscribe(OnShowCameraOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			Cache.BuildAll();
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
			foreach (var element in plan.ElementExtensions.OfType<ElementCamera>().Where(x => x.CameraUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementCamera)
				presenterItem.OverridePainter(new CameraPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			Cache.BuildAll();
		}

		#endregion

		public void Initialize()
		{
			_monitors.Clear();
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowCameraOnPlan(Camera camera)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementExtensions.OfType<ElementCamera>())
					if (element.CameraUID == camera.UID)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}