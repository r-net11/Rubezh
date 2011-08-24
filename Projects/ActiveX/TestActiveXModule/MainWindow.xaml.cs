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
using DevicesModule.ViewModels;
using DevicesModule.Views;

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
            //DataContext = this;
            SelectDeviceCommand = new RelayCommand(OnSelect);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            _currentDeviceViewModel = new CurrentDeviceViewModel();
            _currentDeviceView = new CurrentDeviceView();
            //currentDeviceView.DataContext = _currentDeviceViewModel;
            _currentDeviceViewModel.SelectDevice();
            DataContext = CurrentDeviceViewModel;
            //_mainCurrentDeviceView.DataContext = CurrentDeviceViewModel;
            // currentDeviceView = _currentDeviceViewModel;
        }

        

        CurrentDeviceViewModel _currentDeviceViewModel;
        CurrentDeviceView _currentDeviceView;

        public CurrentDeviceViewModel CurrentDeviceViewModel
        {
            get { return _currentDeviceViewModel; }
        }

        public CurrentDeviceView CurrentDeviceView
        {
            get { return _currentDeviceView; }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelect()
        {
            
        }


        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {

        }
    }
}
