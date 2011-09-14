using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlanDetailsViewModel PlanDetails { get; set; }
        public static PlansViewModel Current;

        public PlansViewModel()
        {
            Current = this;
            MainCanvas = new Canvas();
            PlanDetails = new PlanDetailsViewModel(MainCanvas);
            ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
        }

        public void Initialize()
        {
            Plans = new ObservableCollection<PlanViewModel>();
            BuildTree();
            if (Plans.IsNotNullOrEmpty())
                SelectedPlan = Plans[0];
        }

        void BuildTree()
        {
            if (FiresecManager.PlansConfiguration.Plans.IsNotNullOrEmpty())
            {
                foreach (var plan in FiresecManager.PlansConfiguration.Plans)
                {
                    var planTreeItemViewModel = new ViewModels.PlanViewModel();
                    planTreeItemViewModel.Parent = null;
                    planTreeItemViewModel.Initialize(plan, Plans);
                    planTreeItemViewModel.IsExpanded = true;
                    Plans.Add(planTreeItemViewModel);
                    AddPlan(plan, planTreeItemViewModel);
                }
            }
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanTreeItem)
        {
            if (parentPlan.Children != null)
                foreach (var plan in parentPlan.Children)
                {
                    var planTreeItemViewModel = new ViewModels.PlanViewModel();
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

        public ObservableCollection<PlanViewModel> Plans { get; set; }

        PlanViewModel _selectedPlan;
        public PlanViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                PlanDetails.Initialize(value._plan);
                OnPropertyChanged("SelectedPlan");
            }
        }

        public void OnSelectPlan(string name)
        {
            SelectedPlan = Plans.FirstOrDefault(x => x._plan.Name == name);
        }

        public void ShowDevice(Guid deviceUID)
        {
            foreach (var planViewModel in Plans)
            {
                if (planViewModel._deviceStates.Any(x => x.UID == deviceUID))
                {
                    SelectedPlan = planViewModel;
                    PlanDetails.SelectDevice(deviceUID);
                    break;
                }
            }
        }

        public bool CanZoom
        {
            get { return FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Oper_ShowPlans); }
        }
    }
}