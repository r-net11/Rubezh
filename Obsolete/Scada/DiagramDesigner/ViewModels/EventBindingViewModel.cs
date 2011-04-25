using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DiagramDesigner.Views;
using ControlBase;
using System.Collections.ObjectModel;

namespace DiagramDesigner.ViewModels
{
    public class EventBindingViewModel : INotifyPropertyChanged
    {
        public EventBindingView View { get; set; }

        public void Initialize(List<UserControlBase> UserControls)
        {
            UserControlsItems = new ObservableCollection<UserControlFunctionViewModel>();
            foreach (UserControlBase userControlBase in UserControls)
            {
                UserControlFunctionViewModel userControlFunctionViewModel = new UserControlFunctionViewModel();
                userControlFunctionViewModel.Initialize(userControlBase);
                UserControlsItems.Add(userControlFunctionViewModel);
            }
        }

        ObservableCollection<UserControlFunctionViewModel> userControlsItems;
        public ObservableCollection<UserControlFunctionViewModel> UserControlsItems
        {
            get { return userControlsItems; }
            set
            {
                userControlsItems = value;
                OnPropertyChanged("UserControlsItems");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
