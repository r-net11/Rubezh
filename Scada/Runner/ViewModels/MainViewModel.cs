using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Common;
using System.Reflection;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ControlBase;
using System.Windows;
using System.Windows.Data;
using Runner.ViewModels;
using System.ComponentModel;

namespace Runner
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static MainViewModel Current { get; set; }
        public MainView MainView { get; set; }

        public RelayCommand LoadCommand { get; private set; }
        public DataViewModel dataViewModel { get; set; }
        FormData formData;

        public MainViewModel()
        {
            Current = this;
            LoadCommand = new RelayCommand(OnLoad);
            dataViewModel = new DataViewModel();
        }

        private void OnLoad(object obj)
        {
            Assemblies = new List<Assembly>();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Assembly assembly_1 = Assembly.LoadFile(@"D:/Projects/Scada/Controls/bin/Debug/Controls.dll");
            Assembly assembly_2 = Assembly.LoadFile(@"D:/Projects/Scada/Controls/bin/Debug/CircularGauge.dll");

            Assemblies.Add(assembly_1);
            Assemblies.Add(assembly_2);

            formData = new FormData();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:/Save.bin", FileMode.Open);
            formData = (FormData)formatter.Deserialize(stream);
            stream.Close();

            MainView.RunnerCanvas.Children.Clear();
            foreach (UserControlBase userControlBase in formData.userControlCollection.Controls)
            {
                userControlBase.MouseDown += new System.Windows.Input.MouseButtonEventHandler(userControlBase_MouseDown);
                MainView.RunnerCanvas.Children.Add(userControlBase);
                Canvas.SetLeft(userControlBase, userControlBase.Left);
                Canvas.SetTop(userControlBase, userControlBase.Top);
            }

            SetBinding();
            SetEventBinding();
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assemblies.First(x => x.FullName == args.Name);
        }

        List<Assembly> Assemblies;

        public void SetBinding()
        {
            foreach (BindingItem bindingItem in formData.bindingManager.BindingItems)
            {
                UserControlBase CurrentUserControlBase = null;
                foreach (UIElement element in MainView.RunnerCanvas.Children)
                {
                    if (element is UserControlBase)
                    {
                        UserControlBase userControlBase = element as UserControlBase;
                        if (userControlBase.Id == bindingItem.DestinationId)
                        {
                            CurrentUserControlBase = userControlBase;
                        }
                    }
                }

                DataItem dataItem = formData.dataSource.DataItems.First(x => x.Id == bindingItem.SourceId);

                DependencyProperty dependencyProperty =
                    CurrentUserControlBase.GetType().GetField(bindingItem.DestinationName + "Property").GetValue(CurrentUserControlBase) as DependencyProperty;

                Binding binding = new Binding(bindingItem.SourceName);
                binding.Source = dataItem;
                CurrentUserControlBase.SetBinding(dependencyProperty, binding);

                dataViewModel.Initialize(formData.dataSource.DataItems);
            }
        }

        public void SetEventBinding()
        {
            foreach (EventBindingItem eventBindingItem in formData.eventBindingManager.EventBindingItems)
            {
                UserControlBase SourceUserControlBase = null;
                foreach (UIElement element in MainView.RunnerCanvas.Children)
                {
                    if (element is UserControlBase)
                    {
                        UserControlBase userControlBase = element as UserControlBase;
                        if (userControlBase.Id == eventBindingItem.SourceId)
                        {
                            SourceUserControlBase = userControlBase;
                        }
                    }
                }

                UserControlBase DestinationUserControlBase = null;
                foreach (UIElement element in MainView.RunnerCanvas.Children)
                {
                    if (element is UserControlBase)
                    {
                        UserControlBase userControlBase = element as UserControlBase;
                        if (userControlBase.Id == eventBindingItem.DestinationId)
                        {
                            DestinationUserControlBase = userControlBase;
                        }
                    }
                }

                string functionName = eventBindingItem.DestinationFunctionName;
                string eventName = eventBindingItem.SourceEventName;

                MethodInfo method = DestinationUserControlBase.GetType().GetMethod(functionName);
                EventInfo eventInfo = SourceUserControlBase.GetType().GetEvent(eventName);
                Type type = eventInfo.EventHandlerType;
                Delegate handler = Delegate.CreateDelegate(type, DestinationUserControlBase, method);
                eventInfo.AddEventHandler(SourceUserControlBase, handler);
            }
        }

        void userControlBase_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserControlBase userControlBase = sender as UserControlBase;
            ControlDetails = new ControlDetailsViewModel();
            ControlDetails.Initialize(userControlBase);
        }

        ControlDetailsViewModel controlDetails;
        public ControlDetailsViewModel ControlDetails
        {
            get { return controlDetails; }
            set
            {
                controlDetails = value;
                OnPropertyChanged("ControlDetails");
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
