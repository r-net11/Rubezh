using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
            RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
            AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
            ShowElementsCommand = new RelayCommand(OnShowElements);
            ShowDevicesCommand = new RelayCommand(OnShowDevices);

            DesignerCanvas = new DesignerCanvas();
            PlanDesignerViewModel = new PlanDesignerViewModel();
            PlanDesignerViewModel.DesignerCanvas = DesignerCanvas;

            InitializeCopyPaste();
            InitializeHistory();
            ElementsViewModel = new ElementsViewModel(DesignerCanvas);
            DevicesViewModel = new DevicesViewModel();

            Initialize();
        }

        void Initialize()
        {
            Plans = new ObservableCollection<PlanViewModel>();
            foreach (var plan in FiresecManager.PlansConfiguration.Plans)
            {
                AddPlan(plan, null);
            }

            for (int i = 0; i < Plans.Count; i++)
            {
                CollapseChild(Plans[i]);
                ExpandChild(Plans[i]);
            }

            if (Plans.Count > 0)
            {
                SelectedPlan = Plans[0];
            }
        }

        PlanViewModel AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
        {
            var planViewModel = new PlanViewModel(plan, Plans);
            planViewModel.Parent = parentPlanViewModel;

            var indexOf = Plans.IndexOf(parentPlanViewModel);
            Plans.Insert(indexOf + 1, planViewModel);

            foreach (var childPlan in plan.Children)
            {
                var childPlanViewModel = AddPlan(childPlan, planViewModel);
                planViewModel.Children.Add(childPlanViewModel);
            }

            return planViewModel;
        }

        void CollapseChild(PlanViewModel parentPlanViewModel)
        {
            parentPlanViewModel.IsExpanded = false;
            foreach (var planViewModel in parentPlanViewModel.Children)
            {
                CollapseChild(planViewModel);
            }
        }

        void ExpandChild(PlanViewModel parentPlanViewModel)
        {
            parentPlanViewModel.IsExpanded = true;
            foreach (var planViewModel in parentPlanViewModel.Children)
            {
                ExpandChild(planViewModel);
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
                OnPropertyChanged("SelectedPlan");

                if (value != null)
                {
                    PlanDesignerViewModel.ChangeZoom(1);
                    PlanDesignerViewModel.Save();
                    PlanDesignerViewModel.Initialize(value.Plan);
                    ResetHistory();
                    ElementsViewModel.Update();
                }
            }
        }

        public PlanDesignerViewModel PlanDesignerViewModel { get; set; }

        DesignerCanvas _designerCanvas;
        public DesignerCanvas DesignerCanvas
        {
            get { return _designerCanvas; }
            set
            {
                _designerCanvas = value;
                OnPropertyChanged("DesignerCanvas");
            }
        }

        bool CanAddEditRemove()
        {
            return SelectedPlan != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var planDetailsViewModel = new PlanDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                var plan = planDetailsViewModel.Plan;
                var planViewModel = new PlanViewModel(plan, Plans);
                Plans.Add(planViewModel);
                SelectedPlan = planViewModel;

                FiresecManager.PlansConfiguration.Plans.Add(plan);
                FiresecManager.PlansConfiguration.Update();
                PlansModule.HasChanges = true;
            }
        }

        public RelayCommand AddSubPlanCommand { get; private set; }
        void OnAddSubPlan()
        {
            var planDetailsViewModel = new PlanDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                var plan = planDetailsViewModel.Plan;
                var planViewModel = new PlanViewModel(plan, Plans);

                SelectedPlan.Children.Add(planViewModel);
                SelectedPlan.Plan.Children.Add(plan);
                planViewModel.Parent = SelectedPlan;
                plan.Parent = SelectedPlan.Plan;

                SelectedPlan.Update();
                SelectedPlan = planViewModel;
                FiresecManager.PlansConfiguration.Update();
                PlansModule.HasChanges = true;
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            var selectedPlan = SelectedPlan;
            var parent = SelectedPlan.Parent;
            var plan = SelectedPlan.Plan;

            if (parent == null)
            {
                selectedPlan.IsExpanded = false;
                Plans.Remove(selectedPlan);
                FiresecManager.PlansConfiguration.Plans.Remove(plan);
            }
            else
            {
                parent.IsExpanded = false;
                parent.Plan.Children.Remove(plan);
                parent.Children.Remove(selectedPlan);
                parent.Update();
                parent.IsExpanded = true;
            }

            FiresecManager.PlansConfiguration.Update();
            PlansModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan.Plan);
            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                SelectedPlan.Update();
                DesignerCanvas.Update();
                PlansModule.HasChanges = true;
            }
        }

        ElementsViewModel ElementsViewModel;
        DevicesViewModel DevicesViewModel;

        public RelayCommand ShowElementsCommand { get; private set; }
        void OnShowElements()
        {
            ServiceFactory.UserDialogs.ShowWindow(ElementsViewModel, isTopMost: true, name: "PlanElements");
        }

        public RelayCommand ShowDevicesCommand { get; private set; }
        void OnShowDevices()
        {
            ServiceFactory.UserDialogs.ShowWindow(DevicesViewModel, isTopMost: true, name:"PlanDevices");
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new PlansMenuViewModel(this));
            FiresecManager.UpdatePlansConfiguration();
            DevicesViewModel.Update();

            ServiceFactory.UserDialogs.ResetWindow("PlanElements");
            ServiceFactory.UserDialogs.ResetWindow("PlanDevices");
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);

            ServiceFactory.UserDialogs.HideWindow("PlanElements");
            ServiceFactory.UserDialogs.HideWindow("PlanDevices");
        }
    }
}