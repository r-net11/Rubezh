using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ControlBase;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DiagramDesigner.ViewModels
{
    public class UserControlFunctionViewModel : INotifyPropertyChanged
    {
        public UserControlBase userControlBase { get; private set; }

        public void Initialize(UserControlBase userControlBase)
        {
            this.userControlBase = userControlBase;
            Name = userControlBase.GetType().Name + " - " + userControlBase.Id;

            AvaliableFunctions = new ObservableCollection<FunctionSelection>();

            foreach (MethodInfo methodInfo in userControlBase.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                bool canExecute = (methodInfo.GetCustomAttributes(typeof(FunctionAttribute), false).Count() == 1);
                if (canExecute)
                {
                    FunctionSelection functionSelection = new FunctionSelection();
                    functionSelection.Name = methodInfo.Name;
                    functionSelection.IsSelected = false;
                    AvaliableFunctions.Add(functionSelection);
                }
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        ObservableCollection<FunctionSelection> avaliableFunctions;
        public ObservableCollection<FunctionSelection> AvaliableFunctions
        {
            get { return avaliableFunctions; }
            set
            {
                avaliableFunctions = value;
                OnPropertyChanged("AvaliableFunctions");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FunctionSelection
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
