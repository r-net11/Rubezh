using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Controls;
using System.Collections.ObjectModel;

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
            RootPlanViewModel.Parent = null;
            AddPlan(rootPlan, RootPlanViewModel);
            RootPlanViewModel.Initialize(rootPlan);

            RootPlanViewModels = new ObservableCollection<PlanViewModel>();
            RootPlanViewModels.Add(RootPlanViewModel);

            RootPlanViewModel.Select(null);
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanViewModel)
        {
            foreach (Plan plan in parentPlan.Children)
            {
                PlanViewModel planViewModel = new PlanViewModel();
                planViewModel.Parent = parentPlanViewModel;
                parentPlanViewModel.Children.Add(planViewModel);
                planViewModel.Initialize(plan);
                AddPlan(plan, planViewModel);
            }
        }

        public static MainViewModel Current { get; set; }
        public Canvas MainCanvas { get; set; }

        List<PlanViewModel> AllPlanViewModels;

        public PlanViewModel RootPlanViewModel;

        ObservableCollection<PlanViewModel> rootPlanViewModels;
        public ObservableCollection<PlanViewModel> RootPlanViewModels
        {
            get { return rootPlanViewModels; }
            set
            {
                rootPlanViewModels = value;
                OnPropertyChanged("RootPlanViewModels");
            }
        }

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
