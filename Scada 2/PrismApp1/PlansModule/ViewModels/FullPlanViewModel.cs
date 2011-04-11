using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PlansModule.ViewModels
{
    public class FullPlanViewModel :RegionViewModel
    {
        public FullPlanViewModel()
        {
            Current = this;
            MainCanvas = new Canvas();
            MainCanvas.Background = Brushes.Yellow;
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

            RootPlanViewModel.Select();
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

        public static FullPlanViewModel Current { get; set; }

        Canvas mainCanvas;
        public Canvas MainCanvas
        {
            get { return mainCanvas; }
            set
            {
                mainCanvas = value;
                OnPropertyChanged("MainCanvas");
            }
        }

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

        public override void Dispose()
        {
        }
    }
}
