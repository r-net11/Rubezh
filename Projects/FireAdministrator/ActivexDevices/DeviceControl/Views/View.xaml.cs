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
using System.Windows.Shapes;
using Firesec;

namespace DevicesControl
{
    /// <summary>
    /// Логика взаимодействия для View.xaml
    /// </summary>
    public partial class DeviceControl : UserControl
    {
        public DeviceControl()
        {
            InitializeComponent();
            ViewModel viewModel = new ViewModel();
            this.DataContext = viewModel;
        }

        string driverId;
        public string DriverId
        {
            get { return driverId; }
            set
            {
                driverId = value;
                string driverName = DriversHelper.GetDriverNameById(driverId);
                ViewModel.Current.SelectedDeviceViewModel = ViewModel.Current.DevicesViewModel.FirstOrDefault(x => x.Id == driverName);
            }
        }

        string stateId;
        public string StateId
        {
            get { return stateId; }
            set
            {
                stateId = value;
                ViewModel.Current.SelectedStateViewModel = ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(x => x.Id == stateId);
            }
        }

        List<string> additionalStates;
        public List<string> AdditionalStates
        {
            get { return additionalStates; }
            set
            {
                additionalStates = value;
                StateViewModel stateViewModel;
                for (int i = 0; i < additionalStates.Count; i++)
                {
                    stateViewModel = ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(x => x.Id == additionalStates[i]);
                    stateViewModel.IsChecked = true;
                }
            }
        }
    }
}
