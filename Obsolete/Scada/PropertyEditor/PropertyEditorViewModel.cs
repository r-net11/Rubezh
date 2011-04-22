using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using ControlBase;
using PropertyEditor.EditorViewModel;
using System.Collections.ObjectModel;

namespace PropertyEditor
{
    public class PropertyEditorViewModel : INotifyPropertyChanged
    {
        public PropertyEditorView view;
        public PropertyEditorViewModel()
        {
        }

        ObservableCollection<BaseEditorViewModel> properties;
        public ObservableCollection<BaseEditorViewModel> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                OnPropertyChanged("Properties");
            }
        }

        ObservableCollection<EventViewModel> events;
        public ObservableCollection<EventViewModel> Events
        {
            get { return events; }
            set
            {
                events = value;
                OnPropertyChanged("Events");
            }
        }

        public string EditingType { get; set; }

        public void Initialize(object EditingObject)
        {
            Properties = new ObservableCollection<BaseEditorViewModel>();
            Events = new ObservableCollection<EventViewModel>();

            EditingType = EditingObject.GetType().Name;

            foreach (PropertyInfo propertyInfo in EditingObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.DeclaringType.IsSubclassOf(typeof(UserControlBase)) || propertyInfo.DeclaringType == typeof(UserControlBase))
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite)
                    {
                        BaseEditorViewModel baseEditorViewModel = null;

                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            baseEditorViewModel = new EnumEditorViewModel(EditingObject, propertyInfo.Name);
                        }
                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            baseEditorViewModel = new StringEditorViewModel(EditingObject, propertyInfo.Name);
                        }
                        else if (propertyInfo.PropertyType == typeof(int))
                        {
                            baseEditorViewModel = new IntEditorViewModel(EditingObject, propertyInfo.Name);
                        }
                        else if (propertyInfo.PropertyType == typeof(double))
                        {
                            baseEditorViewModel = new DoubleEditorViewModel(EditingObject, propertyInfo.Name);
                        }
                        else if (propertyInfo.PropertyType == typeof(bool))
                        {
                            baseEditorViewModel = new BoolEditorViewModel(EditingObject, propertyInfo.Name);
                        }
                        else if (propertyInfo.PropertyType == typeof(Color))
                        {
                            baseEditorViewModel = new ColorEditorViewModel(EditingObject, propertyInfo.Name);
                        }

                        if (baseEditorViewModel != null)
                            AddProperty(baseEditorViewModel);
                    }
                }
            }

            foreach (EventInfo eventInfo in EditingObject.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public))
            {
                if (eventInfo.DeclaringType.IsSubclassOf(typeof(UserControlBase)) || eventInfo.DeclaringType == typeof(UserControlBase))
                {
                    bool ShowEvent = (eventInfo.GetCustomAttributes(typeof(ControlBase.EventAttribute), false).Count() == 1);
                    if (ShowEvent)
                    {
                        EventViewModel eventViewModel = new EventViewModel();
                        eventViewModel.Initialize(EditingObject, eventInfo.Name);

                        AddEvent(eventViewModel);
                    }
                }
            }
        }

        void AddEvent(EventViewModel eventViewModel)
        {
            Events.Add(eventViewModel);
            return;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Height = 20;
            stackPanel.Orientation = Orientation.Horizontal;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "event";
            textBlock.Width = 100;
            stackPanel.Children.Add(textBlock);

            eventViewModel.View.Width = 100;
            stackPanel.Children.Add(eventViewModel.View);

            view.propertiesStackPanel.Children.Add(stackPanel);
        }

        void AddProperty(BaseEditorViewModel baseEditorViewModel)
        {
            Properties.Add(baseEditorViewModel);
            return;

            //StackPanel stackPanel = new StackPanel();
            //stackPanel.Height = 20;
            //stackPanel.Orientation = Orientation.Horizontal;

            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = baseEditorViewModel.PropertyName;
            //textBlock.Width = 100;
            //stackPanel.Children.Add(textBlock);

            //baseEditorViewModel.View.Width = 100;
            //stackPanel.Children.Add(baseEditorViewModel.View);

            //view.propertiesStackPanel.Children.Add(stackPanel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
