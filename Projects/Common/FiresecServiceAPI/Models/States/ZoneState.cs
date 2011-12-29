using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneState
    {
        public ZoneState()
        {
            StateType = StateType.No;
            RevertColorsForGuardZone = false;
        }

        public bool RevertColorsForGuardZone { get; set; }

        [DataMember]
        public ulong? No { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        public event Action StateChanged;
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
        }
    }
}