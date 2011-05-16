using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecClient;
using System.Windows.Forms;
using Firesec;
using AssadProcessor.Devices;
using System.Diagnostics;

namespace AssadProcessor
{
    public class Controller
    {
        internal static Controller Current { get; private set; }
        Watcher Wather { get; set; }

        public Controller()
        {
            Current = this;
        }

        void StartAssad()
        {
            Services.NetManager.Start();

            Wather = new Watcher();
            Wather.Start();
        }

        public void Start()
        {
            FiresecManager.Start();
            StartAssad();
        }

        internal void AssadConfig(Assad.MHconfigTypeDevice innerDevice, bool all)
        {
            if (all)
            {
                Services.DeviceManager.Config(innerDevice);
            }
        }

        public Assad.DeviceType[] QueryState(Assad.MHqueryStateType content)
        {
            AssadBase device = Configuration.BaseDevices.FirstOrDefault(a => a.DeviceId == content.deviceId);

            if (device != null)
            {
                List<AssadBase> devices = device.FindAllChildren();

                List<Assad.DeviceType> deviceItems = new List<Assad.DeviceType>();
                foreach (AssadBase assadBase in devices)
                {
                    deviceItems.Add(assadBase.GetStates());
                }
                return deviceItems.ToArray();
            }
            return null;
        }

        public Assad.DeviceType QueryAbility(Assad.MHqueryAbilityType content)
        {
            AssadBase device = Configuration.BaseDevices.First(a => a.DeviceId == content.deviceId);
            Assad.DeviceType ability = device.QueryAbility();
            return ability;
        }

        public void AssadExecuteCommand(Assad.MHdeviceControlType controlType)
        {
            AssadDevice assadDevice = Configuration.Devices.First(x => x.DeviceId == controlType.deviceId);
            string commandName = controlType.cmdId;
            if (commandName == "Обновить")
            {
                Assad.CPqueryConfigurationType cPqueryConfigurationType = new Assad.CPqueryConfigurationType();
                NetManager.Send(cPqueryConfigurationType, null);
            }
            else
            {
                Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == assadDevice.Id);
                if (device != null)
                {
                    if (commandName.StartsWith("Сброс "))
                    {
                        commandName = commandName.Replace("Сброс ", "");

                        string driverName = DriversHelper.GetDriverNameById(device.DriverId);
                        if (driverName == "Компьютер")
                        {
                            foreach (Device resetDevice in FiresecManager.CurrentConfiguration.AllDevices)
                            {
                                Firesec.Metadata.drvType driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == resetDevice.DriverId);
                                if (driver.state != null)
                                {
                                    if (driver.state.Any(x => ((x.name == commandName) && (x.manualReset == "1"))))
                                    {
                                        FiresecManager.ResetState(resetDevice, commandName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            FiresecManager.ResetState(device, commandName);
                        }
                    }
                }
            }
        }

        public void ResetAllStates(string deviceId)
        {
            AssadDevice assadDevice = Configuration.Devices.First(x => x.DeviceId == deviceId);
            Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == assadDevice.Id);
            if (device != null)
            {
                string driverName = DriversHelper.GetDriverNameById(device.DriverId);
                Firesec.Metadata.drvType driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                if (driver.state != null)
                {
                    foreach (Firesec.Metadata.stateType state in driver.state)
                    {
                        if (state.manualReset == "1")
                        {
                            FiresecManager.ResetState(device, state.name);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            Services.LogEngine.Save();
            FiresecManager.Stop();
            Services.NetManager.Stop();
        }

        internal bool Ready { get; set; }
    }
}
