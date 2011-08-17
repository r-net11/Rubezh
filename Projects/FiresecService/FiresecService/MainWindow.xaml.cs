using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecService;
using FiresecService.Views;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Current = this;
        }

        public static MainWindow Current { get; private set; }
        public static void AddMessage(string message)
        {
            Current._textBox.Text += message + "\n";
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            FiresecManager.Connect("adm", "");
            FiresecServiceManager.Open();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AlarmTest alarmTest = new AlarmTest();
            alarmTest.Show();
            //string deviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16";
            //string deviceId2 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
            //string deviceId3 = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/799686B6-9CFA-4848-A0E7-B33149AB940C:1.18";
            //var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            //var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
            //var deviceState2 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId2);
            //var deviceState3 = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId3);

            //deviceState.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Fire).IsActive = true;
            //deviceState2.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Attention).IsActive = true;
            //deviceState3.States.FirstOrDefault(x => x.DriverState.StateType == StateType.Failure).IsActive = true;

            //CallbackManager.OnDeviceStateChanged(deviceState);
            //CallbackManager.OnDeviceStateChanged(deviceState2);
            //CallbackManager.OnDeviceStateChanged(deviceState3);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            JournalDataConverter.Convert();
            JournalDataConverter.Select();
        }
    }
}