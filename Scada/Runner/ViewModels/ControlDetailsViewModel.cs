using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ControlBase;
using System.Reflection;
using System.Collections.ObjectModel;
using Common;
using System.Windows;

namespace Runner.ViewModels
{
    public class ControlDetailsViewModel : INotifyPropertyChanged
    {
        public ControlDetailsViewModel()
        {
        }

        public void Initialize(UserControlBase userControlBase)
        {
            Functions = new ObservableCollection<FunctionViewModel>();

            foreach (MethodInfo methodInfo in userControlBase.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                bool canExecute = (methodInfo.GetCustomAttributes(typeof(FunctionAttribute), false).Count() == 1);
                if (canExecute)
                {
                    FunctionViewModel functionViewModel = new FunctionViewModel();
                    functionViewModel.Name = methodInfo.Name;
                    functionViewModel.Initilize(userControlBase, methodInfo.Name);
                    Functions.Add(functionViewModel);
                }
            }
        }

        ObservableCollection<FunctionViewModel> functions;
        public ObservableCollection<FunctionViewModel> Functions
        {
            get { return functions; }
            set
            {
                functions = value;
                OnPropertyChanged("Functions");
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
