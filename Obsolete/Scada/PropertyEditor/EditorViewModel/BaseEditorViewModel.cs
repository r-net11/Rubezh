using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using Common;
using ControlBase;

namespace PropertyEditor
{
    public class BaseEditorViewModel : INotifyPropertyChanged
    {
        public BaseEditorViewModel(object editingObject, string propertyName, UserControl view)
        {
            SetBindingCommand = new RelayCommand(OnSetBinding);

            this.EditingObject = editingObject;
            this.PropertyName = propertyName;

            View = view;
            View.DataContext = this;
            View.LostFocus += (object sender, RoutedEventArgs e) => { Update(); };

            PropertyInfo propertyInfo = editingObject.GetType().GetProperty(PropertyName);
            CanBind = (propertyInfo.GetCustomAttributes(typeof(ControlBase.CanBindAttribute), false).Count() == 1);
        }

        protected object EditingObject { get; private set; }
        public string PropertyName { get; private set; }
        public UserControl View { get; private set; }
        public RelayCommand SetBindingCommand { get; private set; }

        bool canBind = false;
        public bool CanBind
        {
            get { return canBind; }
            set
            {
                canBind = value;
                OnPropertyChanged("CanBind");
            }
        }

        void OnSetBinding(object parameter)
        {
            Data.Services.OnShowBindingWindow(EditingObject as UserControlBase, PropertyName);
        }

        public virtual void Update()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
