using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            Plans = new ObservableCollection<PlanViewModel>();
        }

        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Plans != null)
            {
                foreach (var plan in FiresecManager.SystemConfiguration.Plans)
                {
                    PlanViewModel planViewModel = new PlanViewModel(plan);
                    Plans.Add(planViewModel);
                }
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
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            PlanDetailsViewModel planDetailsViewModel = new PlanDetailsViewModel();
            planDetailsViewModel.Initialize();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                PlanViewModel planViewModel = new PlanViewModel(planDetailsViewModel.Plan);
                Plans.Add(planViewModel);
            }

        }

        bool CanEditRemove(object obj)
        {
            return SelectedPlan != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FiresecManager.SystemConfiguration.Plans.Remove(SelectedPlan.Plan);
            Plans.Remove(SelectedPlan);
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            PlanDetailsViewModel planDetailsViewModel = new PlanDetailsViewModel();
            planDetailsViewModel.Initialize(SelectedPlan.Plan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                SelectedPlan.Update();
            }
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
