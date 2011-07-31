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
using FiresecClient;
using Infrastructure;
using AlarmModule.Events;
using FiresecAPI.Models;

namespace AlarmModule.Imitator
{
    public partial class AlarmImitatorView : Window
    {
        public AlarmImitatorView()
        {
            InitializeComponent();
        }

        void Set(string id, int state)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.InnerStates.FirstOrDefault(x => x.Priority == state).IsActive = true;
            deviceState.State = new State() { Id = state };
            FiresecEventSubscriber.OnDeviceStateChanged(id);
            deviceState.OnStateChanged();

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == device.ZoneNo);
            zoneState.State = new State() { Id = state };
            FiresecEventSubscriber.OnZoneStateChanged(zoneState.No);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16", 0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:1.2", 0);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:2.16", 0);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17", 1);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:2.17";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsFailure = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:4/641FA899-FAA0-455B-B626-646E5FBE785A:1.74";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsService = true;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:1.3";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsOff = true;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:4/37F13667-BC77-4742-829B-1C43FA404C1F:1.75", 6);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:7/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.16", 6);
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Alarm alarm = new Alarm();
            alarm.AlarmType = AlarmType.Auto;
            alarm.DeviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:7/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.24";
            alarm.Name = "Автоматика задвижки отключена";
            alarm.Time = DateTime.Now.ToString();
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}
