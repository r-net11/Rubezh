using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDriverState
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public XStateType XStateType { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        [DataMember]
        public bool AffectParent { get; set; }

        [DataMember]
        public bool IsManualReset { get; set; }

        [DataMember]
        public bool CanResetOnPanel { get; set; }

        [DataMember]
        public bool IsAutomatic { get; set; }
    }
}