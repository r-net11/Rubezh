using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Controls;

namespace WpfApplication5
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Current = this;
        }

        public void Initialize()
        {
            Plan rootPlan = PlanBuilder.Build();

            RootPlanViewModel = new PlanViewModel();
            RootPlanViewModel.Initialize(rootPlan);
            RootPlanViewModel.Parent = null;
            AddPlan(rootPlan, RootPlanViewModel);

            RootPlanViewModel.DrawPlan();
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanViewModel)
        {
            foreach (Plan plan in parentPlan.Children)
            {
                PlanViewModel planViewModel = new PlanViewModel();
                planViewModel.Initialize(plan);
                planViewModel.Parent = parentPlanViewModel;
                parentPlanViewModel.Children.Add(planViewModel);
                AddPlan(plan, planViewModel);
            }
        }

        public static MainViewModel Current { get; set; }
        public Canvas MainCanvas { get; set; }

        List<PlanViewModel> AllPlanViewModels;
        PlanViewModel RootPlanViewModel { get; set; }

        PlanViewModel selectedPlanViewModel;
        public PlanViewModel SelectedPlanViewModel
        {
            get { return selectedPlanViewModel; }
            set
            {
                selectedPlanViewModel = value;
                OnPropertyChanged("SelectedPlanViewModel");
            }
        }
    }
}
