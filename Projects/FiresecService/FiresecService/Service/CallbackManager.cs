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
                FiresecServiceRunner.MainWindow.AddMessage("Callback error");
                _callbacks.Remove(failedCallback);
            }
        }

        public static void OnNewJournalRecord(JournalRecord journalRecord)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    callback.NewJournalRecord(journalRecord);
                }
                catch
                {
                    _failedCallbacks.Add(callback);
                }
            }

            Clean();
        }

        public static void OnDeviceStateChanged(DeviceState deviceState)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    callback.DeviceStateChanged(deviceState);
                }
                catch
                {
                    _failedCallbacks.Add(callback);
                }
            }

            Clean();
        }

        public static void OnDeviceParametersChanged(DeviceState deviceState)
        {
            _failedCallbacks = new List<IFiresecCallback>();

            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    callback.DeviceParametersChanged(deviceState);
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

            foreach (IFiresecCallback callback in _callbacks)
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