using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecClient
{
    public class CurrentStates
    {
        public List<DeviceState> DeviceStates { get; set; }
        public List<ZoneState> ZoneStates { get; set; }

        public static event Action<Firesec.ReadEvents.journalType> NewJournalEvent;
        public static void OnNewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            if (NewJournalEvent != null)
            {
                NewJournalEvent(journalItem);
            }
        }

        public event Action<string> DeviceStateChanged;
        public void OnDeviceStateChanged(string path)
        {
            if (DeviceStateChanged != null)
                DeviceStateChanged(path);
        }

        public event Action<string> DeviceParametersChanged;
        public void OnDeviceParametersChanged(string path)
        {
            if (DeviceParametersChanged != null)
                DeviceParametersChanged(path);
        }

        public event Action<string> ZoneStateChanged;
        public void OnZoneStateChanged(string zoneNo)
        {
            if (ZoneStateChanged != null)
                ZoneStateChanged(zoneNo);
        }
    }
}
