using System.Collections.ObjectModel;
using System.Windows.Media;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using System.Windows.Controls;
using FiresecAPI.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Windows;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            AddCommand = new RelayCommand(OnAdd);
            AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
            RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
            AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);

            MoveToFrontCommand = new RelayCommand(OnMoveToFront);
            SendToBackCommand = new RelayCommand(OnSendToBack);
            MoveForwardCommand = new RelayCommand(OnMoveForward);
            MoveBackwardCommand = new RelayCommand(OnMoveBackward);

            CopyCommand = new RelayCommand(OnCopy);
            CutCommand = new RelayCommand(OnCut);
            PasteCommand = new RelayCommand(OnPaste);

            DesignerCanvas = new DesignerCanvas();
            PlanDesignerViewModel = new PlanDesignerViewModel();
            PlanDesignerViewModel.DesignerCanvas = DesignerCanvas;

            Initialize();
        }

        public void Initialize()
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
                    PlanDesignerViewModel.Save();
                    PlanDesignerViewModel.Initialize(value.Plan);
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

        public RelayCommand ShowDevicesCommand { get; private set; }
        void OnShowDevices()
        {
            var devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();
            ServiceFactory.UserDialogs.ShowWindow(devicesViewModel);
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
                PlansModule.HasChanges = true;
            }
        }

        public RelayCommand MoveToFrontCommand { get; private set; }
        void OnMoveToFront()
        {
            PlanDesignerViewModel.MoveToFront();
            PlansModule.HasChanges = true;
        }

        public RelayCommand SendToBackCommand { get; private set; }
        void OnSendToBack()
        {
            PlanDesignerViewModel.SendToBack();
            PlansModule.HasChanges = true;
        }

        public RelayCommand MoveForwardCommand { get; private set; }
        void OnMoveForward()
        {
            PlanDesignerViewModel.MoveForward();
            PlansModule.HasChanges = true;
        }

        public RelayCommand MoveBackwardCommand { get; private set; }
        void OnMoveBackward()
        {
            PlanDesignerViewModel.MoveBackward();
            PlansModule.HasChanges = true;
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new PlansMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}