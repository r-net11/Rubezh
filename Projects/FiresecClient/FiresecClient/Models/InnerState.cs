using System.Runtime.Serialization;

namespace FiresecClient.Models
{
    [DataContract]
    public class InnerState
    {
        public State State
        {
            get { return new State(Priority); }
        }

        public bool IsActive { get; set; }

        //[DataMember]
        public string Id { get; set; }

        //[DataMember]
        public string Name { get; set; }

        //[DataMember]
        public bool AffectChildren { get; set; }

        //[DataMember]
        public int Priority { get; set; }

        //[DataMember]
        public bool IsManualReset { get; set; }

        //[DataMember]
        public bool CanResetOnPanel { get; set; }

        //[DataMember]
        public bool IsAutomatic { get; set; }

        //[DataMember]
        public string Code { get; set; }

        public InnerState Copy()
        {
            InnerState innerState = new InnerState();
            innerState.Id = Id;
            innerState.Name = Name;
            innerState.AffectChildren = AffectChildren;
            innerState.Priority = Priority;
            innerState.IsManualReset = IsManualReset;
            innerState.CanResetOnPanel = CanResetOnPanel;
            innerState.IsAutomatic = IsAutomatic;
            innerState.Code = Code;
            return innerState;
        }
    }
}
