using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ClientApi
{
    public class CurrentStates
    {
        public List<DeviceState> DeviceStates { get; set; }
        public List<ZoneState> ZoneStates { get; set; }

        public event Action<DeviceState> DeviceStateChanged;
        public void OnDeviceStateChanged(DeviceState device)
        {
            if (DeviceStateChanged != null)
            {
                DeviceStateChanged(device);
            }
        }

        public event Action<ZoneState> ZoneStateChanged;
        public void OnZoneStateChanged(ZoneState zone)
        {
            if (ZoneStateChanged != null)
            {
                ZoneStateChanged(zone);
            }
        }

        public event Action<Firesec.ReadEvents.journalType> NewJournalEvent;
        public void OnNewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            if (NewJournalEvent != null)
            {
                NewJournalEvent(journalItem);
            }
        }
    }
}
