using System.Collections.ObjectModel;
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
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            Plans = new ObservableCollection<PlanDetailsViewModel>();
        }

        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Plans != null)
            {
                foreach (var plan in FiresecManager.SystemConfiguration.Plans)
                {
                    PlanDetailsViewModel planViewModel = new PlanDetailsViewModel();
                    Plans.Add(planViewModel);
                }
            }
        }

        public ObservableCollection<PlanDetailsViewModel> Plans { get; set; }

        PlanDetailsViewModel _selectedPlan;

        public PlanDetailsViewModel SelectedPlan
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
            var planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                if (planDetailsViewModel.Parent == null)
                {
                    Plans.Add(planDetailsViewModel);
                }
            }
        }

        bool CanEditRemove(object obj)
        {
            return SelectedPlan != null;
        }

        public RelayCommand RemoveCommand { get; private set; }

        bool RemovePlan(ObservableCollection<PlanDetailsViewModel> level)
        {
            bool res = false;
            if (level.Remove(SelectedPlan))
            {
                res = true;
            }
            else
            {
                for (int i = 0; i < level.Count; i++)
                {
                    if (RemovePlan(level[i].Children))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }

        void OnRemove()
        {
            RemovePlan(Plans);
        }

        public RelayCommand EditCommand { get; private set; }

        void OnEdit()
        {
            PlanDetailsViewModel planDetailsViewModel = new PlanDetailsViewModel();
            planDetailsViewModel.Initialize(SelectedPlan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                EditPlan(Plans, planDetailsViewModel);
            }
        }

        bool EditPlan(ObservableCollection<PlanDetailsViewModel> level, PlanDetailsViewModel _newPlan)
        {
            bool res = false;
            int index = level.IndexOf(SelectedPlan);
            if (index != -1)
            {
                level[index].Name = _newPlan.Name;
                level[index].Height = _newPlan.Height;
                level[index].Width = _newPlan.Width;
                SelectedPlan = null;
                res = true;
            }
            else
            {
                for (int i = 0; i < level.Count; i++)
                {
                    index = level[i].Children.IndexOf(SelectedPlan);
                    if (index != -1)
                    {
                        level[index].Name = _newPlan.Name;
                        level[index].Height = _newPlan.Height;
                        level[index].Width = _newPlan.Width;

                        SelectedPlan = null;
                        res = true;
                        break;
                    }
                    else
                    {
                        EditPlan(level[i].Children, _newPlan);
                    }
                }
            }
            return res;
        }

        public override void OnShow()
        {
            PlansContextMenuViewModel plansContextMenuViewModel = new PlansContextMenuViewModel(AddCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(plansContextMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}