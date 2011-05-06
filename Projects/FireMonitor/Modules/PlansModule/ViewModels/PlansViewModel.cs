using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using PlansModule.Events;
using System.Diagnostics;
using PlansModule.Models;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public ObservableCollection<PlanViewModel> Plans { get; set; }

        public PlanDetailsViewModel PlanDetails { get; set; }

        PlanViewModel _selectedPlan;
        public PlanViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                PlanDetails.Initialize(value.plan);
                OnPropertyChanged("SelectedPlan");
            }
        }

        public PlansViewModel()
        {
            MainCanvas = new Canvas();
            PlanDetails = new PlanDetailsViewModel(MainCanvas);
            ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
        }

        public void Initialize()
        {
            Plans = new ObservableCollection<PlanViewModel>();
            BuildTree();
        }

        void BuildTree()
        {
            Plan rootPlan = PlanLoader.Load();

            PlanViewModel planTreeItemViewModel = new ViewModels.PlanViewModel();
            planTreeItemViewModel.Parent = null;
            planTreeItemViewModel.Initialize(rootPlan, Plans);
            planTreeItemViewModel.IsExpanded = true;
            Plans.Add(planTreeItemViewModel);
            AddPlan(rootPlan, planTreeItemViewModel);
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanTreeItem)
        {
            if (parentPlan.Children != null)
                foreach (Plan plan in parentPlan.Children)
                {
                    PlanViewModel planTreeItemViewModel = new ViewModels.PlanViewModel();
                    planTreeItemViewModel.Parent = parentPlanTreeItem;
                    parentPlanTreeItem.Children.Add(planTreeItemViewModel);
                    planTreeItemViewModel.Initialize(plan, Plans);
                    planTreeItemViewModel.IsExpanded = true;
                    Plans.Add(planTreeItemViewModel);
                    AddPlan(plan, planTreeItemViewModel);
                }
        }

        Canvas _mainCanvas;
        public Canvas MainCanvas
        {
            get { return _mainCanvas; }
            set
            {
                _mainCanvas = value;
                OnPropertyChanged("MainCanvas");
            }
        }

        public void OnSelectPlan(string name)
        {
            SelectedPlan = Plans.FirstOrDefault(x => x.plan.Name == name);
        }

        public void ShowDevice(string path)
        {
            foreach (var planViewModel in Plans)
            {
                if (planViewModel.deviceStates.Any(x => x.Path == path))
                {
                    SelectedPlan = planViewModel;
                    PlanDetails.SelectDevice(path);
                    break;
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
