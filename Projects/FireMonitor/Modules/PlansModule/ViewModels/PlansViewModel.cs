using System;
using System.Collections.Generic;
using System.Diagnostics;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using System.Windows.Threading;
using Common;
using Infrustructure.Plans.Painters;
using Infrastructure.Client.Plans;

namespace PlansModule.ViewModels
{
	public class PlansViewModel : ViewPartViewModel
	{
		private bool _initialized;
		public List<IPlanPresenter<Plan>> PlanPresenters { get; private set; }
		public PlanTreeViewModel PlanTreeViewModel { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Subscribe(OnNavigate);
			ServiceFactory.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnFindElementEvent);
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Subscribe(OnRegisterPlanPresenter);
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
			_initialized = false;
			PlanPresenters = new List<IPlanPresenter<Plan>>();
			PlanTreeViewModel = new PlanTreeViewModel(this);
			PlanDesignerViewModel = new PlanDesignerViewModel(this);
			PlanTreeViewModel.SelectedPlanChanged += SelectedPlanChanged;
		}

		public void Initialize()
		{
			using (new TimeCounter("PlansViewModel.Initialize: {0}"))
			{
				using (new TimeCounter("\tPlansViewModel.CacheBrushes: {0}"))
					foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					{
						PainterCache.CacheBrush(plan.BackgroundPixels);
						foreach (var elementBase in PlanEnumerator.Enumerate(plan))
							PainterCache.CacheBrush(elementBase.BackgroundPixels);
					}
				_initialized = false;
				FiresecManager.InvalidatePlans();
				PlanTreeViewModel.Initialize();
				_initialized = true;
				OnSelectedPlanChanged();
			}
		}

		private void OnSelectPlan(Guid planUID)
		{
			PlanTreeViewModel.SelectedPlan = PlanTreeViewModel.FindPlan(planUID);
		}
		private void SelectedPlanChanged(object sender, EventArgs e)
		{
			OnSelectedPlanChanged();
		}
		private void OnSelectedPlanChanged()
		{
			if (_initialized)
				PlanDesignerViewModel.SelectPlan(PlanTreeViewModel.SelectedPlan);
		}

		private void OnRegisterPlanPresenter(IPlanPresenter<Plan> planPresenter)
		{
			if (!PlanPresenters.Contains(planPresenter))
			{
				PlanPresenters.Add(planPresenter);
				if (_initialized)
					PlanTreeViewModel.AddPlanPresenter(planPresenter);
				OnSelectedPlanChanged();
			}
		}

		private void OnShowElement(Guid elementUID)
		{
			foreach (var presenterItem in PlanDesignerViewModel.PresenterItems)
				if (presenterItem.Element.UID == elementUID)
				{
					presenterItem.Navigate();
					PlanDesignerViewModel.Navigate(presenterItem);
				}
		}
		private void OnFindElementEvent(Guid deviceUID)
		{
			foreach (var plan in PlanTreeViewModel.AllPlans)
				foreach (var elementDevice in plan.Plan.ElementUnion)
					if (elementDevice.UID == deviceUID)
					{
						PlanTreeViewModel.SelectedPlan = plan;
						OnShowElement(deviceUID);
						return;
					}
		}
		private void OnNavigate(NavigateToPlanElementEventArgs args)
		{
			Debug.WriteLine("[{0}]Navigation: PlanUID={1}\t\tElementUID={2}", DateTime.Now, args.PlanUID, args.ElementUID);
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
			OnSelectPlan(args.PlanUID);
			OnShowElement(args.ElementUID);
		}
	}
}