using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class PlanDetailsViewModel : DialogContent
    {
        public PlanDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public PlanDetailsViewModel(PlanDetailsViewModel _Parent)
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            _isNew = true;
            Plan = new Plan();
            Title = "Новый план";
            if (_Parent != null) parent = _Parent;
            else parent = null;
        }

        bool _isNew;

        public Plan Plan { get; private set; }

        PlanDetailsViewModel parent;

        public PlanDetailsViewModel Parent
        {
            get { return parent; }
        }

        public void Initialize(PlanDetailsViewModel _Parent)
        {
            _isNew = false;
            Plan = _Parent.Plan;
            Name = _Parent.Name;
            Width = _Parent.Width;
            Height = _Parent.Height;
            Title = "Редактирование плана";
        }

        string _name;
        public string Name
        {
            get { return _name; }
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

        ObservableCollection<PlanDetailsViewModel> _children;
        
        public ObservableCollection<PlanDetailsViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }

        void Save()
        {
            Plan.Name = Name;
            Plan.Height = Height;
            Plan.Width = Width;
            Plan.Children = null;
            if (parent != null)
            {
                if (parent.Children == null) parent.Children = new ObservableCollection<PlanDetailsViewModel>();
                parent.Children.Add(this);
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
