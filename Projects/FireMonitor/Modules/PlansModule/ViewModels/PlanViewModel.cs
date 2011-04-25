using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlansModule.Views;
using System.Diagnostics;
using PlansModule.Events;
using PlansModule.Models;
using System.Reflection;
using Firesec;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : RegionViewModel
    {
        public PlanViewModel()
        {
            Children = new ObservableCollection<PlanViewModel>();
            ParentPlans = new ObservableCollection<PlanViewModel>();
            SelectCommand = new RelayCommand(Select);
        }

        public PlanViewModel Parent { get; set; }
        public ObservableCollection<PlanViewModel> Children { get; set; }

        public Plan plan { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }

        public ObservableCollection<PlanViewModel> ParentPlans { get; set; }

        Canvas MainCanvas { get; set; }

        public List<ElementSubPlanViewModel> SubPlans { get; set; }
        public List<ElementZoneViewModel> Zones { get; set; }
        public List<ElementDeviceViewModel> Devices { get; set; }


        public RelayCommand SelectCommand { get; private set; }
        public void Select()
        {
            //IsSelected = true;
            isSelected = true;
            OnPropertyChanged("IsSelected");
            DrawPlan();
            ExpandParent();
            ResetView();
        }

        void ResetView()
        {
            if (CanvasView.Current != null)
            {
                CanvasView.Current.Reset();
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                if (isSelected)
                {
                    ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(Name);
                }
            }
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        void ExpandParent()
        {
            if (Parent != null)
            {
                Parent.IsExpanded = true;
                //ExpandParent();
            }
        }

        public void Initialize(Plan plan, Canvas mainCanvas)
        {
            MainCanvas = mainCanvas;
            this.plan = plan;
            Name = plan.Name;
            Caption = plan.Caption;
            UpdateParentPlans();
        }

        public void DrawPlan()
        {
            SubPlans = new List<ElementSubPlanViewModel>();
            Zones = new List<ElementZoneViewModel>();
            Devices = new List<ElementDeviceViewModel>();

            MainCanvas.Children.Clear();

            MainCanvas.Width = plan.Width;
            MainCanvas.Height = plan.Height;

            MainCanvas.Background = CreateBrush(plan.BackgroundSource);

            foreach (ElementSubPlan elementSubPlan in plan.ElementSubPlans)
            {
                ElementSubPlanViewModel subPlanViewModel = new ElementSubPlanViewModel();
                SubPlans.Add(subPlanViewModel);
                subPlanViewModel.Initialize(elementSubPlan, MainCanvas);
            }

            foreach (ElementZone elementZone in plan.ElementZones)
            {
                ElementZoneViewModel zonePlanViewModel = new ElementZoneViewModel();
                Zones.Add(zonePlanViewModel);
                zonePlanViewModel.Initialize(elementZone, MainCanvas);
            }

            foreach (ElementDevice elementDevice in plan.ElementDevices)
            {
                ElementDeviceViewModel planDeviceViewModel = new ElementDeviceViewModel();
                Devices.Add(planDeviceViewModel);
                planDeviceViewModel.Initialize(elementDevice, MainCanvas, this);
            }

            UpdateSelfState();
            //UpdateSubPlans();
        }

        string selfState = "Норма";
        public string SelfState
        {
            get { return selfState; }
            set { selfState = value; }
        }

        string state = "Норма";
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        public void UpdateSelfState()
        {
            int minPriority = 7;
            foreach (ElementDeviceViewModel planDeviceViewModel in Devices)
            {
                int priority = StateHelper.NameToPriority(planDeviceViewModel.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            SelfState = StateHelper.GetState(minPriority);

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = StateHelper.NameToPriority(SelfState);

            foreach (PlanViewModel planViewModel in Children)
            {
                int priority = StateHelper.NameToPriority(planViewModel.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            State = StateHelper.GetState(minPriority);

            UpdateSubPlans();

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }

        public void UpdateSubPlans()
        {
            foreach (ElementSubPlanViewModel subPlan in SubPlans)
            {
                PlanViewModel planViewModel = Children.FirstOrDefault(x => x.Name == subPlan.Name);
                if (planViewModel != null)
                {
                    subPlan.Update(planViewModel.State);
                }
            }
        }

        void UpdateParentPlans()
        {
            ParentPlans = new ObservableCollection<PlanViewModel>();
            foreach (PlanViewModel parentPlan in AllParents)
            {
                ParentPlans.Add(parentPlan);
            }
        }

        public List<PlanViewModel> AllParents
        {
            get
            {
                if (Parent == null)
                {
                    return new List<PlanViewModel>();
                }

                List<PlanViewModel> allParents = Parent.AllParents;
                allParents.Add(Parent);
                return allParents;
            }
        }

        public override void Dispose()
        {
        }

        ImageBrush CreateBrush(string source)
        {
            ImageBrush imageBrush = new ImageBrush();
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string relativePath = "../../../Data/" + source;
            Uri uri = new Uri(System.IO.Path.Combine(executablePath, relativePath));
            imageBrush.ImageSource = new BitmapImage(uri);
            return imageBrush;
        }
    }
}
