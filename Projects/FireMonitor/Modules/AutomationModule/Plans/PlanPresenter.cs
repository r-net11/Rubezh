using AutomationModule.Plans.Designer;
using Common;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Presenter;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.Plans
{
	class PlanPresenter : IPlanPresenter<Plan, XStateClass>
	{
		public static MapSource Cache { get; private set; }

		public PlanPresenter()
		{
			Cache = new MapSource();
			Cache.Add(() => ClientManager.SystemConfiguration.AutomationConfiguration.Procedures);
			Cache.BuildAll();
			ServiceFactory.Events.GetEvent<ShowProcedureOnPlanEvent>().Subscribe(OnShowProcedureOnPlan);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
		}

		#region IPlanPresenter<Plan> Members

		public void SubscribeStateChanged(Plan plan, Action callBack)
		{
			Cache.BuildAll();
		}

		public NamedStateClass GetNamedStateClass(Plan plan)
		{
			return new NamedStateClass();
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementExtensions.OfType<ElementProcedure>().Where(x => x.ProcedureUID != Guid.Empty))
				yield return element;
		}

		public void RegisterPresenterItem(PresenterItem presenterItem)
		{
			if (presenterItem.Element is ElementProcedure)
				presenterItem.OverridePainter(new ProcedurePainter(presenterItem));
		}
		public void ExtensionAttached()
		{
			Cache.BuildAll();
		}

		#endregion

		public void Initialize()
		{
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
		}

		private void OnShowProcedureOnPlan(Procedure procedure)
		{
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementExtensions.OfType<ElementProcedure>())
					if (element.ProcedureUID == procedure.Uid)
					{
						ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
						return;
					}
		}
	}
}