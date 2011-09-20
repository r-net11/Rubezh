using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            MainCanvas = new Canvas() { Background = new SolidColorBrush(Colors.Red) };
            Initialize();
        }

        void Initialize()
        {
            if (FiresecManager.PlansConfiguration.Plans.IsNotNullOrEmpty())
            {
                Plans = new ObservableCollection<PlanViewModel>(
                    FiresecManager.PlansConfiguration.Plans.Select(plan => new PlanViewModel(plan))
                );

                SelectedPlan = Plans[0];
            }
            else
            {
                Plans = new ObservableCollection<PlanViewModel>();
            }

            AddCommand = new RelayCommand(OnAdd);
            AddSubCommand = new RelayCommand(OnAddSub, CanAddSub);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
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

        public static void Update()
        {
            //OnPropertyChanged("Plans");
        }

        ObservableCollection<PlanViewModel> _plans;
        public ObservableCollection<PlanViewModel> Plans
        {
            get { return _plans; }
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
                if (PlanCanvasView.Current != null)
                    PlanCanvasView.Current.ChangeSelectedPlan(SelectedPlan.Plan);
            }
        }

        bool RemovePlan(ObservableCollection<PlanViewModel> plans, PlanViewModel selectedPlan)
        {
            if (plans.Remove(SelectedPlan))
            {
                FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
                return true;
            }
            else
            {
                foreach (var plan in plans)
                {
                    if (plans.Remove(selectedPlan))
                    {
                        FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
                        return true;
                    }
                    else
                    {
                        RemovePlan(plan.Children, SelectedPlan);
                    }
                }
            }
            return false;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var planDetailsViewModel = (SelectedPlan != null) ? new PlanDetailsViewModel(SelectedPlan.Plan) : new PlanDetailsViewModel();
            planDetailsViewModel.Initialize();

            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                if (SelectedPlan != null)
                {
                    SelectedPlan.Children.Add(new PlanViewModel(planDetailsViewModel.Plan));
                }
                else
                {
                    Plans.Add(new PlanViewModel(planDetailsViewModel.Plan));
                }
            }
        }

        public RelayCommand AddSubCommand { get; private set; }
        void OnAddSub()
        {
            var subPlanDetailsViewModel = new SubPlanDetailsViewModel(SelectedPlan.Plan);
            if (ServiceFactory.UserDialogs.ShowModalWindow(subPlanDetailsViewModel))
            {
                var elementSubPlanViewModel = new SubPlanViewModel(subPlanDetailsViewModel.Parent, subPlanDetailsViewModel.ElementSubPlan);
                SelectedPlan.ElementSubPlans.Add(elementSubPlanViewModel);
            }
        }

        bool CanAddSub()
        {
            return SelectedPlan != null;
        }

        bool CanEditRemove()
        {
            return SelectedPlan != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                SelectedPlan.Plan.Name = planDetailsViewModel.Plan.Name;
                var plan = SelectedPlan.Plan;
                while (plan.Parent != null) plan = plan.Parent;
                int index = FiresecManager.PlansConfiguration.Plans.IndexOf(plan);
                FiresecManager.PlansConfiguration.Plans[index] = plan;
                SelectedPlan.Update();
            }
        }

        public override void OnShow()
        {
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