using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DriverState
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        [DataMember]
        public bool IsManualReset { get; set; }

        [DataMember]
        public bool CanResetOnPanel { get; set; }

        [DataMember]
        public bool IsAutomatic { get; set; }

        public DriverState Copy()
        {
            var driverState = new DriverState();
            driverState.Id = Id;
            driverState.Name = Name;
            driverState.AffectChildren = AffectChildren;
            driverState.StateType = StateType;
            driverState.IsManualReset = IsManualReset;
            driverState.CanResetOnPanel = CanResetOnPanel;
            driverState.IsAutomatic = IsAutomatic;
            driverState.Code = Code;
            return driverState;
        }
    }
}