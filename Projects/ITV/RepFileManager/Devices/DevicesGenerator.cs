using System.Collections.Generic;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public static class DevicesGenerator
    {
        public static List<repositoryModuleDevice> Generate()
        {
            var devices = new List<repositoryModuleDevice>();

            var computerDevice = new ComputerDevice();
            devices.Add(computerDevice.Device);

            var communicationDevice = new CommunicationDevice();
            devices.Add(communicationDevice.Device);

            var panelDevice = new PanelDevice();
            devices.Add(panelDevice.Device);

            foreach (var driver in Helper.RealDevices)
            {
                var repDevice = new RealDevice();
                repDevice.Initialize(driver);
                devices.Add(repDevice.Device);
            }

            return devices;
        }
    }
}
