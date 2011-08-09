using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class InnerState
    {
        public State State
        {
            get { return new State() { Id = StateClassId }; }
        }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int StateClassId { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        [DataMember]
        public bool IsManualReset { get; set; }

        [DataMember]
        public bool CanResetOnPanel { get; set; }

        [DataMember]
        public bool IsAutomatic { get; set; }

        public InnerState Copy()
        {
            InnerState innerState = new InnerState();
            innerState.Id = Id;
            innerState.Name = Name;
            innerState.AffectChildren = AffectChildren;
            innerState.StateClassId = StateClassId;
            innerState.IsManualReset = IsManualReset;
            innerState.CanResetOnPanel = CanResetOnPanel;
            innerState.IsAutomatic = IsAutomatic;
            innerState.Code = Code;
            return innerState;
        }
    }
}
