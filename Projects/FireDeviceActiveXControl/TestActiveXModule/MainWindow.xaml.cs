using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure.Common;
using CurrentDeviceModule.ViewModels;
using CurrentDeviceModule.Views;
using FiresecClient;

namespace TestActiveXModule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FiresecManager.Connect("adm", "");
            SelectDeviceCommand = new RelayCommand(OnSelect);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            currentDeviceViewModel = new CurrentDeviceViewModel();
        }

        CurrentDeviceViewModel currentDeviceViewModel;

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelect()
        {
            DeviceTreeViewModel deviceTreeViewModel = new DeviceTreeViewModel();
            deviceTreeViewModel.Initialize();
            DeviceTreeView deviceTreeView = new DeviceTreeView();
            deviceTreeView.DataContext = deviceTreeViewModel;
            deviceTreeViewModel.Closing += deviceTreeView.Close;
            deviceTreeView.Show();
        }


        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
        }
    }
}
