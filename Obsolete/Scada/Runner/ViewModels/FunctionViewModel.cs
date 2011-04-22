using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using System.Windows;
using ControlBase;
using System.Reflection;

namespace Runner.ViewModels
{
    public class FunctionViewModel : INotifyPropertyChanged
    {
        public RelayCommand CallFunction { get; private set; }
        UserControlBase userControlBase;

        public FunctionViewModel()
        {
            CallFunction = new RelayCommand(OnCallFunction);
        }

        public void Initilize(UserControlBase userControlBase, string functionName)
        {
            this.userControlBase = userControlBase;
            Name = functionName;
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

        void OnCallFunction(object obj)
        {
            MethodInfo methodInfo = userControlBase.GetType().GetMethod(Name);
            methodInfo.Invoke(userControlBase, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
