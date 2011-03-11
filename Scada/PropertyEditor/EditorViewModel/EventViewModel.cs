using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using PropertyEditor;
using Common;
using System.Windows;
using ControlBase;

namespace PropertyEditor.EditorViewModel
{
    public class EventViewModel : INotifyPropertyChanged
    {
        public RelayCommand SetBindingCommand { get; private set; }
        public UserControl View { get; private set; }
        object EditingObject;

        public EventViewModel()
        {
            SetBindingCommand = new RelayCommand(OnSetBinding);

            View = new EventView();
            View.DataContext = this;
        }

        public void Initialize(object editingObject, string eventName)
        {
            EditingObject = editingObject;
            EventName = eventName;
        }

        void OnSetBinding(object obj)
        {
            Data.Services.OnShowEventBindingWindow(EditingObject as UserControlBase, EventName);
        }

        string eventName;
        public string EventName
        {
            get { return eventName; }
            set
            {
                eventName = value;
                OnPropertyChanged("EventName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
