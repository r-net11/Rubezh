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

    public partial class DeviceControl : UserControl
    {
        public DeviceControl()
        {
            InitializeComponent();
            viewModel = new ViewModel();
            this.DataContext = viewModel;
        }
        public ViewModel viewModel;
        string driverId;
        public string DriverId
        {
            get { return driverId; }
            set
            {
                driverId = value;
                string driverName = DriversHelper.GetDriverNameById(driverId);
                viewModel.SelectedDeviceViewModel = ViewModel.DevicesViewModel.FirstOrDefault(x => x.Id == driverName);
            }
        }

        string stateId;
        public string StateId
        {
            get { return stateId; }
            set
            {
                stateId = value;
                viewModel.SelectedStateViewModel = viewModel.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(x => x.Id == stateId);
                ViewModel.Current.SelectedStateViewModel = viewModel.SelectedStateViewModel;
            }
        }

        List<string> additionalStates;
        public List<string> AdditionalStates
        {
            get { return additionalStates; }
            set
            {
                additionalStates = value;
                List<StateViewModel> StatesViewModel = new List<StateViewModel>();
                for (int i = 0; i < additionalStates.Count; i++ )
                    StatesViewModel.Add(viewModel.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(x => x.id == additionalStates[i]));
                viewModel.AdditionalStatesViewModel = StatesViewModel;
            }
        }
    }
}
