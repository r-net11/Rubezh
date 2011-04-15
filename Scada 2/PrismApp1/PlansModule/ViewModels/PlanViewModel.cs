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
            MainCanvas.Children.Clear();

            MainCanvas.Width = plan.Width;
            MainCanvas.Height = plan.Height;

            ImageBrush backgroundImageBrush = new ImageBrush();
            backgroundImageBrush.ImageSource = new BitmapImage(new Uri(plan.BackgroundSource, UriKind.Absolute));
            MainCanvas.Background = backgroundImageBrush;

            foreach (SubPlan subPlan in plan.SubPlans)
            {
                SubPlanViewModel subPlanViewModel = new SubPlanViewModel();
                subPlanViewModel.Initialize(subPlan, MainCanvas);
            }

            foreach (PlanZone planZone in plan.PlanZones)
            {
                ZonePlanViewModel zonePlanViewModel = new ZonePlanViewModel();
                zonePlanViewModel.Initialize(planZone, MainCanvas);
            }

            foreach (PlanDevice planDevice in plan.PlanDevices)
            {
                PlanDeviceViewModel planDeviceViewModel = new PlanDeviceViewModel();
                planDeviceViewModel.Initialize(planDevice, MainCanvas);
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
    }
}
