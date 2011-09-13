using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecService
{
    public static class CallbackManager
    {
        static CallbackManager()
        {
            _callbacks = new List<IFiresecCallback>();
        }

        static List<IFiresecCallback> _callbacks;
        static List<IFiresecCallback> _failedCallbacks;

        public static void Add(IFiresecCallback callback)
        {
            _callbacks.Add(callback);
        }

        public static void Remove(IFiresecCallback callback)
        {
            _callbacks.Remove(callback);
        }

        static void Clean()
        {
            foreach (var failedCallback in _failedCallbacks)
            {
                _callbacks.Remove(failedCallback);
            }
        }

        public static void OnNewJournalRecord(JournalRecord journalRecord)
        {
            lock (FiresecService.locker)
            {
                _failedCallbacks = new List<IFiresecCallback>();

                foreach (var callback in _callbacks)
                {
                    try
                    {
                        callback.NewJournalRecord(journalRecord);
                        FiresecServiceRunner.MainWindow.AddMessage("New Journal Event");
                    }
                    catch
                    {
                        _failedCallbacks.Add(callback);
                    }
                }

                Clean();
            }
        }

        public static void OnDeviceStatesChanged(List<DeviceState> deviceStates)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (var callback in _callbacks)
            {
                try
                {
                    callback.DeviceStateChanged(deviceStates);
                }
                catch
                {
                    _failedCallbacks.Add(callback);
                }
            }

            Clean();
        }

        public static void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (var callback in _callbacks)
            {
                try
                {
                    callback.DeviceParametersChanged(deviceParameters);
                }
                catch
                {
                    _failedCallbacks.Add(callback);
                }
            }

            Clean();
        }

        public static void OnZoneStateChanged(ZoneState zoneState)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (var callback in _callbacks)
            {
                try
                {
                    callback.ZoneStateChanged(zoneState);
                }
                catch
                {
                    _failedCallbacks.Add(callback);
                }
            }

            Clean();
        }
    }
}