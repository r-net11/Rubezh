using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrastructure.Common;
using System.Windows;
using Infrastructure.Common.Windows;

namespace PlansModule.ViewModels
{
	public class PlansViewModel : ViewPartViewModel
	{
		bool _initialized;
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
			var planNavigationWidth = RegistrySettingsHelper.GetDouble("Monitor.Plans.SplitterDistance");
			//if (planNavigationWidth == 0)
			//    planNavigationWidth = 100;
			//if (!IsPlanTreeVisible)
			//    planNavigationWidth = 0;
			PlanNavigationWidth = new GridLength(planNavigationWidth, GridUnitType.Pixel);
			ApplicationService.ShuttingDown += () =>
			{
				RegistrySettingsHelper.SetDouble("Monitor.Plans.SplitterDistance", PlanNavigationWidth.Value);
			};
		}

		public void Initialize()
		{
			using (new TimeCounter("PlansViewModel.Initialize: {0}"))
			{
				using (new TimeCounter("\tPlansViewModel.CacheBrushes: {0}"))
					foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					{
						PainterCache.CacheBrush(plan);
						foreach (var elementBase in PlanEnumerator.Enumerate(plan))
							PainterCache.CacheBrush(elementBase);
					}
				_initialized = false;
				FiresecManager.InvalidatePlans();
				PlanTreeViewModel.Initialize();
				_initialized = true;
			}
		}

		void OnSelectPlan(Guid planUID)
		{
			var newPlan = PlanTreeViewModel.FindPlan(planUID);
			if (PlanTreeViewModel.SelectedPlan == newPlan)
				PlanDesignerViewModel.Update();
			else
				PlanTreeViewModel.SelectedPlan = newPlan;
		}
		void SelectedPlanChanged(object sender, EventArgs e)
		{
			OnSelectedPlanChanged();
		}
		void OnSelectedPlanChanged()
		{
			if (_initialized)
				PlanDesignerViewModel.SelectPlan(PlanTreeViewModel.SelectedPlan);
		}

		void OnRegisterPlanPresenter(IPlanPresenter<Plan> planPresenter)
		{
			if (!PlanPresenters.Contains(planPresenter))
			{
				PlanPresenters.Add(planPresenter);
				if (_initialized)
					PlanTreeViewModel.AddPlanPresenter(planPresenter);
				OnSelectedPlanChanged();
			}
		}

		void OnShowElement(Guid elementUID)
		{
			foreach (var presenterItem in PlanDesignerViewModel.PresenterItems)
				if (presenterItem.Element.UID == elementUID)
				{
					presenterItem.Navigate();
					PlanDesignerViewModel.Navigate(presenterItem);
				}
		}
		void OnFindElementEvent(List<Guid> deviceUIDs)
		{
			foreach (var plan in PlanTreeViewModel.AllPlans)
				if (plan.PlanFolder == null)
					foreach (var elementDevice in plan.Plan.ElementUnion)
						if (deviceUIDs.Contains(elementDevice.UID))
						{
							PlanTreeViewModel.SelectedPlan = plan;
							OnShowElement(elementDevice.UID);
							return;
						}
		}
		void OnNavigate(NavigateToPlanElementEventArgs args)
		{
			Debug.WriteLine("[{0}]Navigation: PlanUID={1}\t\tElementUID={2}", DateTime.Now, args.PlanUID, args.ElementUID);
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
			OnSelectPlan(args.PlanUID);
			OnShowElement(args.ElementUID);
		}

		public bool IsPlanTreeVisible
		{
			get { return !GlobalSettingsHelper.GlobalSettings.Monitor_HidePlansTree; }
		}

		public override void OnShow()
		{
			base.OnShow();
			foreach (var planPresenter in PlanPresenters)
				planPresenter.ExtensionAttached();
			PlanTreeViewModel.Select();
		}

		GridLength _planNavigationWidth;
		public GridLength PlanNavigationWidth
		{
			get { return _planNavigationWidth; }
			set
			{
				_planNavigationWidth = value;
				OnPropertyChanged(() => PlanNavigationWidth);
			}
		}
	}
}