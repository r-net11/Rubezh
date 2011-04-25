using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using System.Threading;
using FiresecClient;
using System.Windows.Forms;
using Firesec;

namespace AssadProcessor
{
    public class Controller
    {
        internal static Controller Current { get; private set; }
        AssadWatcher assadWather { get; set; }

        public Controller()
        {
            Current = this;
        }

        void StartAssad()
        {
            Services.NetManager.Start();

            assadWather = new AssadWatcher();
            assadWather.Start();
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
                AssadServices.AssadDeviceManager.Config(innerDevice);
                if (waitForConfiguration)
                {
                    waitForConfiguration = false;

                    CurrentConfiguration currentConfiguration = AssadToServiceConverter.Convert();
                    FiresecManager.SetNewConfig(currentConfiguration);
                }
            }
        }

        bool waitForConfiguration = false;

        void SetNewConfiguration()
        {
            waitForConfiguration = true;
            Assad.CPqueryConfigurationType cPqueryConfigurationType = new Assad.CPqueryConfigurationType();
            NetManager.Send(cPqueryConfigurationType, null);
        }

        public void AssadExecuteCommand(Assad.MHdeviceControlType controlType)
        {
            AssadBase assadDevice = AssadConfiguration.Devices.First(x => x.DeviceId == controlType.deviceId);
            string commandName = controlType.cmdId;
            if (commandName == "Записать Конфигурацию")
            {
                // провести первичную проверку валидности
                if (AssadConfiguration.IsValid)
                {
                    SetNewConfiguration();
                }
                else
                {
                    MessageBox.Show("Неправильная конфигурация");
                }
            }
            else
            {
                Device device = Helper.ConvertDevice(assadDevice);
                if (commandName.StartsWith("Сброс "))
                {
                    commandName = commandName.Replace("Сброс ", "");

                    string driverName = DriversHelper.GetDriverNameById(device.DriverId);
                    if (driverName == "Компьютер")
                    {
                        foreach (Device resetDevice in FiresecManager.CurrentConfiguration.AllDevices)
                        {
                            Firesec.Metadata.drvType driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == resetDevice.DriverId);
                            if(driver.state != null)
                            {
                                if (driver.state.Any(x=>((x.name == commandName) && (x.manualReset == "1"))))
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

        public void ResetAllStates(string deviceId)
        {
            AssadBase assadDevice = AssadConfiguration.Devices.First(x => x.DeviceId == deviceId);
            Device device = Helper.ConvertDevice(assadDevice);
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

        public void Stop()
        {
            Services.LogEngine.Save();
            FiresecManager.Stop();
            Services.NetManager.Stop();
        }

        bool ready = false;
        internal bool Ready
        {
            get { return ready; }
            set
            {
                ready = true;
            }
        }
    }
}
