using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            AddSubCommand = new RelayCommand(OnAddSub, CanAddSub);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            Plans = new ObservableCollection<PlanViewModel>();
            //            plansMenuViewModel = new PlansMenuViewModel(AddCommand, EditCommand, RemoveCommand);
            //            PlansContextMenuViewModel plansContextMenuViewModel = new PlansContextMenuViewModel(AddCommand, EditCommand, RemoveCommand);
        }

        public void Initialize()
        {
            if (FiresecManager.PlansConfiguration.Plans != null)
            {
                foreach (var plan in FiresecManager.PlansConfiguration.Plans)
                {
                    var planViewModel = new PlanViewModel(plan);
                    Plans.Add(planViewModel);
                    if (plan.Children != null)
                    {
                        BuildTree(plan.Children, planViewModel);
                    }
                }
                // Добавил проверку на пустой список
                if (Plans.Count > 0)
                {
                    SelectedPlan = Plans[0];
                }
            }
        }

        public void BuildTree(List<Plan> _plans, PlanViewModel parent)
        {
            foreach (var plan in _plans)
            {
                var planViewModel = new PlanViewModel(plan);
                planViewModel.AddChild(parent, planViewModel);
                plan.Parent = parent.Plan;
                if (plan.Children != null)
                {
                    BuildTree(plan.Children, planViewModel);
                }
            }
        }

        public ObservableCollection<PlanViewModel> _plans;
        public ObservableCollection<PlanViewModel> Plans
        {
            get
            {
                return _plans;
            }
            set
            {
                _plans = value;
                OnPropertyChanged("Plans");
            }
        }

        PlanViewModel _selectedPlan;

        public PlanViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                OnPropertyChanged("SelectedPlan");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            PlanDetailsViewModel planDetailsViewModel = null;
            if (SelectedPlan != null)
            {
                planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan.Plan);
                planDetailsViewModel.Initialize();
            }
            else
            {
                planDetailsViewModel = new PlanDetailsViewModel();
                planDetailsViewModel.Initialize();
            }

            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                if (SelectedPlan == null)
                {
                    var planViewModel = new PlanViewModel(planDetailsViewModel.Plan);
                    Plans.Add(planViewModel);
                }
                else
                {
                    var planViewModel = new PlanViewModel(planDetailsViewModel.Plan);
                    planViewModel.AddChild(SelectedPlan, planViewModel);
                }
            }
        }

        public RelayCommand AddSubCommand { get; private set; }
        void OnAddSub()
        {
            var subPlanDetailsViewModel = new SubPlanDetailsViewModel(SelectedPlan.Plan);
            subPlanDetailsViewModel.Initialize();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(subPlanDetailsViewModel);
            if (result)
            {
                var elementSubPlanViewModel = new SubPlanViewModel(subPlanDetailsViewModel.Parent, subPlanDetailsViewModel.SubPlan);
            }

        }

        bool CanAddSub(object obj)
        {
            return SelectedPlan != null;
        }


        bool CanEditRemove(object obj)
        {
            return SelectedPlan != null;
        }

        public RelayCommand RemoveCommand { get; private set; }

        bool RemovePlan(ObservableCollection<PlanViewModel> _plans, PlanViewModel _selectedPlan)
        {
            bool res = false;
            if (_plans.Remove(SelectedPlan))
            {
                FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
                res = true;
            }
            else
            {
                foreach (var plan in _plans)
                {
                    if (_plans.Remove(_selectedPlan))
                    {
                        FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
                        res = true;
                        break;
                    }
                    else
                    {
                        RemovePlan(plan.Children, SelectedPlan);
                    }
                }
            }
            return res;
        }

        void OnRemove()
        {
            if (SelectedPlan.Plan.Parent != null)
            {
                Plan plan = SelectedPlan.Plan.Parent;
                plan.Children.Remove(SelectedPlan.Plan);
                while (plan.Parent != null) plan = plan.Parent;
                int index = FiresecManager.PlansConfiguration.Plans.IndexOf(plan);
                FiresecManager.PlansConfiguration.Plans[index] = plan;
            }
            else
            {
                FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
            }

            RemovePlan(Plans, SelectedPlan);
            SelectedPlan.Update();
            SelectedPlan = null;
        }

        public RelayCommand EditCommand { get; private set; }

        void OnEdit()
        {
            var planDetailsViewModel = new PlanDetailsViewModel();
            planDetailsViewModel.Initialize(SelectedPlan.Plan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                SelectedPlan.Plan.Name = planDetailsViewModel.Plan.Name;
                var plan = SelectedPlan.Plan;
                while (plan.Parent != null) plan = plan.Parent;
                int index = FiresecManager.PlansConfiguration.Plans.IndexOf(plan);
                FiresecManager.PlansConfiguration.Plans[index] = plan;
                SelectedPlan.Update();
                //SelectedPlan = null;
            }
        }

        public override void OnShow()
        {
            //SelectedPlan = SelectedPlan;
            var plansContextMenuViewModel = new PlansContextMenuViewModel(AddCommand, AddSubCommand, EditCommand, RemoveCommand);
            var plansMenuViewModel = new PlansMenuViewModel(AddCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(plansMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}