using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryState
    {
        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public List<LibraryFrame> Frames { get; set; }
    }
}