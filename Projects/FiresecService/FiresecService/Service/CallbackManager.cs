using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.ViewModels;

namespace FiresecService
{
    public static class CallbackManager
    {
        static CallbackManager()
        {
            _serviceInstances = new List<FiresecService>();
        }

        static List<FiresecService> _serviceInstances;
        static List<FiresecService> _failedServiceInstances;

        public static void Add(FiresecService firesecService)
        {
            _serviceInstances.Add(firesecService);
        }

        public static void Remove(FiresecService firesecService)
        {
            _serviceInstances.Remove(firesecService);
        }

        static void Clean()
        {
            try
            {
                foreach (var failedServiceInstance in _failedServiceInstances)
                {
                    var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.FiresecServiceUID == failedServiceInstance.FiresecServiceUID);
                    MainViewModel.Current.RemoveConnection(connectionViewModel);

                    _serviceInstances.Remove(failedServiceInstance);
                }
            }
            catch { ;}
        }

        static void SafeCall(Action<FiresecService> action)
        {
            lock (FiresecService.Locker)
            {
                _failedServiceInstances = new List<FiresecService>();
                foreach (var serviceInstance in _serviceInstances)
                {
                    if (serviceInstance.IsSubscribed)
                        try
                        {
                            action(serviceInstance);
                        }
                        catch
                        {
                            _failedServiceInstances.Add(serviceInstance);
                        }
                }

                Clean();
            }
        }

        public static void OnNewJournalRecord(JournalRecord journalRecord)
        {
            SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); });
        }

        public static void OnDeviceStatesChanged(List<DeviceState> deviceStates)
        {
            SafeCall((x) => { x.Callback.DeviceStateChanged(deviceStates); });
        }

        public static void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
        {
            SafeCall((x) => { x.Callback.DeviceParametersChanged(deviceParameters); });
        }

        public static void OnZoneStateChanged(ZoneState zoneState)
        {
            SafeCall((x) => { x.Callback.ZoneStateChanged(zoneState); });
        }

        public static void OnConfigurationChanged()
        {
            SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); });
        }

        public static bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            lock (FiresecService.Locker)
            {
                _failedServiceInstances = new List<FiresecService>();
                foreach (var serviceInstance in _serviceInstances)
                {
                    try
                    {
                        var result = serviceInstance.FiresecCallbackService.Progress(stage, comment, percentComplete, bytesRW);
                        return result;
                    }
                    catch
                    {
                        _failedServiceInstances.Add(serviceInstance);
                    }
                }

                Clean();
                return true;
            }
        }
    }
}