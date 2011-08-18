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
using FiresecAPI.Models;

namespace FiresecService.Views
{
    /// <summary>
    /// Логика взаимодействия для AlarmTest.xaml
    /// </summary>
    public partial class AlarmTest : Window
    {
        public AlarmTest()
        {
            InitializeComponent();
        }

        private void rbDevice_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton device = (sender as RadioButton);
        }

        private void rbStateType_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton stateType = (sender as RadioButton);
            StateChanged(stateType.Content.ToString());
        }

        string deviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16";
        StateType _stateType { get; set; }

        public void StateChanged(string state)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
            foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            {
                if (Enum.GetName(typeof(StateType), stateType) == state)
                {
                    var deviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == _stateType);
                    if (deviceDriverState != null)
                    {
                        deviceDriverState.IsActive = false;
                    }
                    _stateType = stateType;
                    var newDeviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.StateType == _stateType);
                    if (newDeviceDriverState != null)
                    {
                        newDeviceDriverState.IsActive = true;
                    }
                    deviceDriverState = null;
                    newDeviceDriverState = null;
                    CallbackManager.OnDeviceStateChanged(deviceState);
                }
            }
        }
        //string deviceId2 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
        //string deviceId3 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/799686B6-9CFA-4848-A0E7-B33149AB940C:1.18";
        //var deviceState2 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId2);
        //var deviceState3 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId3);

        //deviceState.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Fire).IsActive = true;
        //deviceState2.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Attention).IsActive = true;
        //deviceState3.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Failure).IsActive = true;

        //CallbackManager.OnDeviceStateChanged(deviceState);
        //CallbackManager.OnDeviceStateChanged(deviceState2);
        //CallbackManager.OnDeviceStateChanged(deviceState3);
    }
}
