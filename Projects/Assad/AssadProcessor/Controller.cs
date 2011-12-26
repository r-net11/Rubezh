using System.Collections.Generic;
using System.Linq;
using AssadProcessor.Devices;
using FiresecAPI.Models;
using FiresecClient;

namespace AssadProcessor
{
    public class Controller
    {
        internal static Controller Current { get; private set; }
        Watcher _watcher;

        public Controller()
        {
            Current = this;
        }

        public void Start()
        {
            FiresecManager.Connect("net.tcp://localhost:8000/FiresecService/", "adm", "");

            Services.NetManager.Start();
            _watcher = new Watcher();
            _watcher.Start();
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
            var assadBase = Configuration.BaseDevices.FirstOrDefault(a => a.DeviceId == content.deviceId);
            if (assadBase != null)
            {
                List<AssadBase> devices = assadBase.FindAllChildren();

                var deviceItems = new List<Assad.DeviceType>();
                foreach (var childAssadBase in devices)
                {
                    deviceItems.Add(childAssadBase.GetStates());
                }
                return deviceItems.ToArray();
            }
            return null;
        }

        public Assad.DeviceType QueryAbility(Assad.MHqueryAbilityType content)
        {
            var device = Configuration.BaseDevices.First(a => a.DeviceId == content.deviceId);
            Assad.DeviceType ability = device.QueryAbility();
            return ability;
        }

        public void AssadExecuteCommand(Assad.MHdeviceControlType controlType)
        {
            var assadDevice = Configuration.Devices.First(x => x.DeviceId == controlType.deviceId);
            string commandName = controlType.cmdId;
            if (commandName == "Обновить")
            {
                var cPqueryConfigurationType = new Assad.CPqueryConfigurationType();
                NetManager.Send(cPqueryConfigurationType, null);
            }
            else
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == assadDevice.Id);
                if (device != null)
                {
                    if (commandName.StartsWith("Сброс "))
                    {
                        commandName = commandName.Replace("Сброс ", "");

                        if (device.Driver.DriverType == DriverType.Computer)
                        {
                            foreach (var resetDevice in FiresecManager.DeviceConfiguration.Devices)
                            {
                                if (resetDevice.Driver.States.Any(x => ((x.Name == commandName) && (x.IsManualReset))))
                                {
                                    var resetItem = new ResetItem();
                                    resetItem.DeviceUID = resetDevice.UID;
                                    resetItem.StateNames = new List<string>() { commandName };
                                    FiresecManager.ResetStates(new List<ResetItem>() { resetItem });
                                }
                            }
                        }
                        else
                        {
                            var resetItem = new ResetItem();
                            resetItem.DeviceUID = device.UID;
                            resetItem.StateNames = new List<string>() { commandName };
                            FiresecManager.ResetStates(new List<ResetItem>() { resetItem });
                        }
                    }
                }
            }
        }

        public void ResetAllStates(string deviceId)
        {
            var assadDevice = Configuration.Devices.First(x => x.DeviceId == deviceId);
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == assadDevice.Id);
            if (device != null)
            {
                foreach (var state in device.Driver.States)
                {
                    if (state.IsManualReset)
                    {
                        var resetItem = new ResetItem();
                        resetItem.DeviceUID = device.UID;
                        resetItem.StateNames = new List<string>() { state.Name };
                        FiresecManager.ResetStates(new List<ResetItem>() { resetItem });
                    }
                }
            }
        }

        public void Stop()
        {
            Services.LogEngine.Save();
            FiresecManager.Disconnect();
            Services.NetManager.Stop();
        }

        internal bool Ready { get; set; }
    }
}