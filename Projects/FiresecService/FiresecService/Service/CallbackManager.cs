using System.Collections.Generic;
using FiresecAPI.Models;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecService.ViewModels;
using FiresecAPI;

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
            //SafeCall((x) => { x.Callback.NewJournalRecord(journalRecord); });
            SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); });

            //lock (FiresecService.Locker)
            //{
            //    _failedServiceInstances = new List<FiresecService>();
            //    foreach (var serviceInstance in _serviceInstances)
            //    {
            //        if (serviceInstance.IsSubscribed)
            //            try
            //            {
            //                serviceInstance.Callback.NewJournalRecord(journalRecord);
            //            }
            //            catch
            //            {
            //                _failedServiceInstances.Add(serviceInstance);
            //            }
            //    }

            //    Clean();
            //}
        }

        public static void OnDeviceStatesChanged(List<DeviceState> deviceStates)
        {
            SafeCall((x) => { x.Callback.DeviceStateChanged(deviceStates); });

            //_failedServiceInstances = new List<FiresecService>();
            //foreach (var serviceInstance in _serviceInstances)
            //{
            //    if (serviceInstance.IsSubscribed)
            //        try
            //        {
            //            serviceInstance.Callback.DeviceStateChanged(deviceStates);
            //        }
            //        catch
            //        {
            //            _failedServiceInstances.Add(serviceInstance);
            //        }
            //}

            //Clean();
        }

        public static void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
        {
            SafeCall((x) => { x.Callback.DeviceParametersChanged(deviceParameters); });

            //_failedServiceInstances = new List<FiresecService>();
            //foreach (var serviceInstance in _serviceInstances)
            //{
            //    if (serviceInstance.IsSubscribed)
            //        try
            //        {
            //            serviceInstance.Callback.DeviceParametersChanged(deviceParameters);
            //        }
            //        catch
            //        {
            //            _failedServiceInstances.Add(serviceInstance);
            //        }
            //}

            //Clean();
        }

        public static void OnZoneStateChanged(ZoneState zoneState)
        {
            SafeCall((x) => { x.Callback.ZoneStateChanged(zoneState); });

            //_failedServiceInstances = new List<FiresecService>();

            //foreach (var serviceInstance in _serviceInstances)
            //{
            //    if (serviceInstance.IsSubscribed)
            //        try
            //        {
            //            serviceInstance.Callback.ZoneStateChanged(zoneState);
            //        }
            //        catch
            //        {
            //            _failedServiceInstances.Add(serviceInstance);
            //        }
            //}

            //Clean();
        }

        public static void OnConfigurationChanged()
        {
            SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); });

            //_failedServiceInstances = new List<FiresecService>();

            //foreach (var serviceInstance in _serviceInstances)
            //{
            //    if (serviceInstance.IsSubscribed)
            //        try
            //        {
            //            serviceInstance.FiresecCallbackService.ConfigurationChanged();
            //        }
            //        catch
            //        {
            //            _failedServiceInstances.Add(serviceInstance);
            //        }
            //}

            //Clean();
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

                        //if (serviceInstance.ContinueProgress)
                        //{
                        //    serviceInstance.ContinueProgress = true;
                        //}
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