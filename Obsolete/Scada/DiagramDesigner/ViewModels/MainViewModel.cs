using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Common;
using PropertyEditor;
using ControlBase;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using DiagramDesigner.ViewModels;

namespace DiagramDesigner
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Current = this;
            SaveCommand = new RelayCommand(OnSave);
            SetBindingCommand = new RelayCommand(OnSetBinding);
        }

        public void Initialize()
        {
            LoadToolBarControls();
            formData = new FormData();
            formData.dataSource = SampleData.GetSampleData();
            formData.bindingManager = new BindingManager();
            formData.bindingManager.BindingItems = new List<BindingItem>();
            formData.eventBindingManager = new EventBindingManager();
            formData.eventBindingManager.EventBindingItems = new List<EventBindingItem>();
            Services.ShowBindingWindow += new Action<UserControlBase, string>(ShowBindingWindow);
            Services.ShowEventBindingWindow += new Action<UserControlBase, string>(ShowEventBindingWindow);
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand SetBindingCommand { get; private set; }

        public FormData formData { get; set; }

        public static MainViewModel Current { get; set; }
        public MainView MainView { get; set; }

        public void ShowProperty(object item)
        {
            PropertyEditorView propertyEditorView = new PropertyEditorView();
            PropertyEditorViewModel propertyEditorViewModel = new PropertyEditorViewModel();
            propertyEditorView.DataContext = propertyEditorViewModel;
            propertyEditorViewModel.view = propertyEditorView;
            propertyEditorViewModel.Initialize(item);
            MainView.PropertyStackPanel.Children.Clear();
            MainView.PropertyStackPanel.Children.Add(propertyEditorView);
        }

        private void OnSave(object obj)
        {
            UserControlCollection userControlCollection = new UserControlCollection();
            userControlCollection.Controls = new List<UserControlBase>();

            foreach (UIElement element in MainView.MyDesignerCanvas.Children)
            {
                if (element is DesignerItem)
                {
                    DesignerItem designerItem = element as DesignerItem;
                    if (designerItem.Content is UserControlBase)
                    {
                        UserControlBase userControlBase = designerItem.Content as UserControlBase;
                        userControlBase.Height = designerItem.Height;
                        userControlBase.Width = designerItem.Width;
                        userControlBase.Left = Canvas.GetLeft(designerItem);
                        userControlBase.Top = Canvas.GetTop(designerItem);
                        userControlCollection.Controls.Add(userControlBase);
                    }
                }
            }

            formData.userControlCollection = userControlCollection;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:/Save.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, formData);
            stream.Close();
        }

        private void LoadToolBarControls()
        {
            Assembly assembly1 = Assembly.LoadFile(@"D:/Projects/Scada/Controls/bin/Debug/CircularGauge.dll");
            Assembly assembly2 = Assembly.LoadFile(@"D:/Projects/Scada/Controls/bin/Debug/Controls.dll");
            Type[] types = assembly2.GetTypes();
            foreach (Type type in types)
            {
                object o = Activator.CreateInstance(type);
                if (o is UserControlBase)
                {
                    UserControlBase userControlBase = o as UserControlBase;
                    ToolBoxItem toolBoxItem = new ToolBoxItem();
                    toolBoxItem.UserControlBase = userControlBase;
                    MainView.Palette.Children.Add(toolBoxItem);
                }
            }
        }

        void ShowBindingWindow(UserControlBase source, string propertyName)
        {
            Views.BindingView BindingView = new DiagramDesigner.Views.BindingView();
            ViewModels.BindingViewModel BindingViewModel = new DiagramDesigner.ViewModels.BindingViewModel();
            BindingViewModel.Initialize(formData.dataSource.DataItems);
            BindingView.DataContext = BindingViewModel;
            BindingViewModel.BindingView = BindingView;
            BindingView.ShowDialog();

            Data.DataItem SelectedDataItem = null;
            if (BindingViewModel.SelectedDataItem != null)
            {
                SelectedDataItem = BindingViewModel.SelectedDataItem.DataItem;
            }

            BindingItem bindingItem = new BindingItem();
            bindingItem.DestinationId = source.Id;
            bindingItem.SourceId = SelectedDataItem.Id;
            bindingItem.DestinationName = propertyName;
            bindingItem.SourceName = "DataValue";
            formData.bindingManager.BindingItems.Add(bindingItem);
        }

        void ShowEventBindingWindow(UserControlBase source, string eventName)
        {
            List<UserControlBase> UserControls = MainView.MyDesignerCanvas.GetAllControls();

            Views.EventBindingView eventBindingView = new DiagramDesigner.Views.EventBindingView();
            ViewModels.EventBindingViewModel eventBindingViewModel = new DiagramDesigner.ViewModels.EventBindingViewModel();
            eventBindingViewModel.Initialize(UserControls);
            eventBindingView.DataContext = eventBindingViewModel;
            eventBindingViewModel.View = eventBindingView;
            eventBindingView.ShowDialog();

            foreach(UserControlFunctionViewModel userControlFunctionViewModel in eventBindingViewModel.UserControlsItems)
            {
                foreach(FunctionSelection functionSelection in userControlFunctionViewModel.AvaliableFunctions)
                {
                    if (functionSelection.IsSelected)
                    {
                        EventBindingItem eventBindingItem = new EventBindingItem();
                        eventBindingItem.DestinationFunctionName = functionSelection.Name;
                        eventBindingItem.DestinationId = userControlFunctionViewModel.userControlBase.Id;
                        eventBindingItem.SourceId = source.Id;
                        eventBindingItem.SourceEventName = eventName;
                        formData.eventBindingManager.EventBindingItems.Add(eventBindingItem);
                    }
                }
            }
        }

        private void OnSetBinding(object obj)
        {
        }
    }
}
