using System;
using System.Linq;
using System.Windows;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace AlarmModule.Imitator
{
    public partial class AlarmImitatorView : Window
    {
        public AlarmImitatorView()
        {
            InitializeComponent();
        }

        void Set(string id, StateType stateType)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.States.FirstOrDefault(x => x.DriverState.StateType == stateType).IsActive = true;
            FiresecEventSubscriber.OnDeviceStateChanged(id);
            deviceState.OnStateChanged();

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == device.ZoneNo);
            zoneState.StateType = stateType;
            FiresecEventSubscriber.OnZoneStateChanged(zoneState.No);
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.16", 0);
        }

        void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:1.2", 0);
        }

        void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:2.16", 0);
        }

        void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17", (StateType) 1);
        }

        void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:2.17";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsFailure = true;
        }

        void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:4/641FA899-FAA0-455B-B626-646E5FBE785A:1.74";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsService = true;
        }

        void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string id = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:2/37F13667-BC77-4742-829B-1C43FA404C1F:1.3";
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            deviceState.IsOff = true;
        }

        void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:4/37F13667-BC77-4742-829B-1C43FA404C1F:1.75", (StateType) 6);
        }

        void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Set("F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:7/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.16", (StateType) 6);
        }

        void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var alarm = new Alarm();
            alarm.AlarmType = AlarmType.Auto;
            alarm.DeviceId = "F8340ECE-C950-498D-88CD-DCBABBC604F3:0/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:7/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.24";
            alarm.Name = "Автоматика задвижки отключена";
            alarm.Time = DateTime.Now.ToString();
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);
        }
    }
}