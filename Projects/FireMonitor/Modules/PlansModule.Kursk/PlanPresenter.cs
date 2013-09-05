using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using PlansModule.Kursk.Designer;

namespace PlansModule.Kursk
{
	class PlanPresenter : IPlanPresenter<Plan>
	{
		private Dictionary<Plan, PlanMonitor> _monitors;
		public PlanPresenter()
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			_monitors = new Dictionary<Plan, PlanMonitor>();
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			var monitor = new PlanMonitor(plan, callBack);
			_monitors.Add(plan, monitor);
		}

		public int GetState(Plan plan)
		{
			return (int)(_monitors.ContainsKey(plan) ? _monitors[plan].GetState() : StateType.No);
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementExtensions.OfType<ElementRectangleTank>().Where(x => x.XDeviceUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementRectangleTank)
				presenterItem.OverridePainter(new TankPainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			Helper.BuildMap();
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