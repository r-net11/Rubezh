using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class PlansViewModel : ViewPartViewModel
	{
		bool _initialized;
		public List<IPlanPresenter<Plan, XStateClass>> PlanPresenters { get; private set; }
		public PlanTreeViewModel PlanTreeViewModel { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }

		public PlansViewModel(List<IPlanPresenter<Plan, XStateClass>> planPresenters)
		{
			PlanPresenters = planPresenters;
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Subscribe(OnNavigate);
			ServiceFactory.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnFindElementEvent);
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
			_initialized = false;
			PlanTreeViewModel = new PlanTreeViewModel(this);
			PlanDesignerViewModel = new PlanDesignerViewModel(this);
			PlanTreeViewModel.SelectedPlanChanged += SelectedPlanChanged;

			var isNotFirstTime = RegistrySettingsHelper.GetBool("Monitor.Plans.IsNotFirstTime");
			var planNavigationWidth = RegistrySettingsHelper.GetDouble("Monitor.Plans.SplitterDistance");
			if (!isNotFirstTime && planNavigationWidth == 0)
			{
				planNavigationWidth = 100;
			}
			PlanNavigationWidth = new GridLength(planNavigationWidth, GridUnitType.Pixel);
			ApplicationService.ShuttingDown += () =>
			{
				RegistrySettingsHelper.SetDouble("Monitor.Plans.SplitterDistance", PlanNavigationWidth.Value);
				RegistrySettingsHelper.SetBool("Monitor.Plans.IsNotFirstTime", true);
			};
		}

		public void Initialize()
		{
			_initialized = false;
			PlanTreeViewModel.Initialize();
			_initialized = true;
			OnSelectedPlanChanged();
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
			if (PlanTreeViewModel.AllPlans == null)
				return;

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
			//Debug.WriteLine("[{0}]Navigation: PlanUID={1}\t\tElementUID={2}", DateTime.Now, args.PlanUID, args.ElementUID);
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