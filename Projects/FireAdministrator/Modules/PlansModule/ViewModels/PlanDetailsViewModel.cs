using System.Collections.ObjectModel;
using System.Collections.Generic;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;

namespace PlansModule.ViewModels
{
    public class PlanDetailsViewModel : DialogContent
    {
        public PlanDetailsViewModel()
        {
            Title = "Новый план";
            Plan = new Plan();
            Parent = null;
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public PlanDetailsViewModel(Plan parent)
        {
            Title = "Новый план";
            Plan = new Plan();
            if (parent.Children == null) parent.Children = new List<FiresecAPI.Models.Plan>();
            parent.Children.Add(Plan);
            Plan.Parent = parent;
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Initialize()
        {
            _isNew = true;
        }
        public void Initialize(Plan plan)
        {
            _isNew = false;
            Name = plan.Name;
            Width = plan.Width;
            Height = plan.Height;
            Title = "Редактирование плана";
        }
        public void Initialize(PlanViewModel plan)
        {
            _isNew = false;
            Name = plan.Name;
            Width = plan.Plan.Width;
            Height = plan.Plan.Height;
            Title = "Редактирование плана";
        }
        
        bool _isNew; 
        public Plan Plan { get; private set; }
        public Plan Parent { get; private set; }

        string _name;
        public string Name
        {
            get 
            { 
                return _name; 
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        void Save()
        {
            Plan.Name = Name;
            Plan.Height = Height;
            Plan.Width = Width;
            if (_isNew)
            {
                Plan plan = Plan;
                while (plan.Parent != null) plan = plan.Parent;

                if (FiresecManager.PlansConfiguration.Plans == null) FiresecManager.PlansConfiguration.Plans = new List<FiresecAPI.Models.Plan>();
                int index = FiresecManager.PlansConfiguration.Plans.IndexOf(plan);
                if (index == -1)
                {
                    FiresecManager.PlansConfiguration.Plans.Add(plan);
                }
                else
                {
                    FiresecManager.PlansConfiguration.Plans[index] = plan;
                }
            }
        }


        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (Name != null)
            {
                Save();
                Close(true);
            }
            else Close(false);

        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}