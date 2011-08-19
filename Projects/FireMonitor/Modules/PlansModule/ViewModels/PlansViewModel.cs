using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using FiresecClient;

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
            if (Plans.IsNotNullOrEmpry())
                SelectedPlan = Plans[0];
        }

        void BuildTree()
        {
            Plan rootPlan = PlanLoader.Load();

            var planTreeItemViewModel = new ViewModels.PlanViewModel();
            planTreeItemViewModel.Parent = null;
            planTreeItemViewModel.Initialize(rootPlan, Plans);
            planTreeItemViewModel.IsExpanded = true;
            Plans.Add(planTreeItemViewModel);
            AddPlan(rootPlan, planTreeItemViewModel);
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

        public void ShowDevice(string id)
        {
            foreach (var planViewModel in Plans)
            {
                if (planViewModel._deviceStates.Any(x => x.Id == id))
                {
                    SelectedPlan = planViewModel;
                    PlanDetails.SelectDevice(id);
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