using System.Linq;
using System.Windows;
using FiresecService;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            FiresecManager.Connect("adm", "");
            FiresecServiceManager.Open();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string deviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16";
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId);

            deviceState.States.FirstOrDefault(x => x.DriverState.StateClassId == 0).IsActive = true;

            CallbackManager.OnDeviceStateChanged(deviceState);
        }
    }
}
