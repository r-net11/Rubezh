using System.Collections.ObjectModel;
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
            Plan.Children = null;
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public PlanDetailsViewModel(Plan parent)
        {
            Title = "Новый план";
            Plan = new Plan();
            Plan.Children = null;
            Parent = parent;
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
                FiresecManager.SystemConfiguration.Plans.Add(Plan);
            }
        }

        public void Update()
        {
            //OnPropertyChanged("Plan");
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}