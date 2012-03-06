using System.Collections.Generic;
using FiresecAPI.Models;
using System;
using System.Diagnostics;

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
                foreach (var failedCallback in _failedServiceInstances)
                {
                    _failedServiceInstances.Remove(failedCallback);
                }
            }
            catch { ;}
        }

        public static void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            lock (FiresecService.Locker)
            {
                _failedServiceInstances = new List<FiresecService>();
                foreach (var serviceInstance in _serviceInstances)
                {
                    try
                    {
                        serviceInstance.FiresecCallbackService.Progress(stage, comment, percentComplete, bytesRW);

                        if (serviceInstance.ContinueProgress)
                        {
                            serviceInstance.ContinueProgress = true;
                        }
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
            lock (FiresecService.Locker)
            {
                _failedServiceInstances = new List<FiresecService>();
                foreach (var serviceInstance in _serviceInstances)
                {
                    if (serviceInstance.IsSubscribed)
                        try
                        {
                            serviceInstance.Callback.NewJournalRecord(journalRecord);
                        }
                        catch
                        {
                            _failedServiceInstances.Add(serviceInstance);
                        }
                }

                Clean();
            }
        }

        public static void OnDeviceStatesChanged(List<DeviceState> deviceStates)
        {
            _failedServiceInstances = new List<FiresecService>();
            foreach (var serviceInstance in _serviceInstances)
            {
                if (serviceInstance.IsSubscribed)
                    try
                    {
                        serviceInstance.Callback.DeviceStateChanged(deviceStates);
                    }
                    catch
                    {
                        _failedServiceInstances.Add(serviceInstance);
                    }
            }

            Clean();
        }

        public static void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
        {
            _failedServiceInstances = new List<FiresecService>();
            foreach (var serviceInstance in _serviceInstances)
            {
                if (serviceInstance.IsSubscribed)
                    try
                    {
                        serviceInstance.Callback.DeviceParametersChanged(deviceParameters);
                    }
                    catch
                    {
                        _failedServiceInstances.Add(serviceInstance);
                    }
            }

            Clean();
        }

        public static void OnZoneStateChanged(ZoneState zoneState)
        {
            _failedServiceInstances = new List<FiresecService>();

            foreach (var serviceInstance in _serviceInstances)
            {
                if (serviceInstance.IsSubscribed)
                    try
                    {
                        serviceInstance.Callback.ZoneStateChanged(zoneState);
                    }
                    catch
                    {
                        _failedServiceInstances.Add(serviceInstance);
                    }
            }

            Clean();
        }

        public static void OnConfigurationChanged()
        {
            _failedServiceInstances = new List<FiresecService>();

            foreach (var serviceInstance in _serviceInstances)
            {
                if (serviceInstance.IsSubscribed)
                    try
                    {
                        serviceInstance.FiresecCallbackService.ConfigurationChanged();
                    }
                    catch
                    {
                        _failedServiceInstances.Add(serviceInstance);
                    }
            }

            Clean();
        }
    }
}