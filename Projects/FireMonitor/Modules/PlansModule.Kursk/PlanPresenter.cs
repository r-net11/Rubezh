using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using PlansModule.Kursk.Painters;

namespace PlansModule.Kursk
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }
		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add(() => GKManager.Devices);

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

		public XStateClass GetState(Plan plan)
		{
			return _monitors.ContainsKey(plan) ? _monitors[plan].GetState() : XStateClass.No;
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementExtensions.OfType<ElementRectangleTank>().Where(x => x.DeviceUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementRectangleTank)
				presenterItem.OverridePainter(new TankPainter(presenterItem));
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
	}
}