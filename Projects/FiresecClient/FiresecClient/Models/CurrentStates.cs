using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Firesec;

namespace FiresecClient.Models
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
        public void OnDeviceStateChanged(string id)
        {
            if (DeviceStateChanged != null)
                DeviceStateChanged(id);
        }

        public event Action<string> DeviceParametersChanged;
        public void OnDeviceParametersChanged(string id)
        {
            if (DeviceParametersChanged != null)
                DeviceParametersChanged(id);
        }

        public event Action<string> ZoneStateChanged;
        public void OnZoneStateChanged(string zoneNo)
        {
            if (ZoneStateChanged != null)
                ZoneStateChanged(zoneNo);
        }
    }
}
