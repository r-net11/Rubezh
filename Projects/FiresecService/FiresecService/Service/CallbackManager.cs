using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI;

namespace FiresecService
{
    public static class CallbackManager
    {
        static CallbackManager()
        {
            _callbacks = new List<IFiresecCallback>();
        }

        static List<IFiresecCallback> _callbacks;

        public static void Add(IFiresecCallback callback)
        {
            _callbacks.Add(callback);
        }

        public static void Remove(IFiresecCallback callback)
        {
            _callbacks.Remove(callback);
        }

        public static void OnNewJournalItem(JournalItem journalItem)
        {
            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.NewJournalItem(journalItem);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnDeviceStateChanged(DeviceState deviceState)
        {
            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.DeviceStateChanged(deviceState);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnDeviceParametersChanged(DeviceState deviceState)
        {
            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.DeviceParametersChanged(deviceState);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnZoneStateChanged(ZoneState zoneState)
        {
            foreach (IFiresecCallback callback in _callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.ZoneStateChanged(zoneState);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }
    }
}
