using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryState
    {
        public LibraryState()
        {
            Frames = new List<LibraryFrame>();
            Frames.Add(new LibraryFrame() { Id = 0 });
            Layer = 0;
        }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public List<LibraryFrame> Frames { get; set; }

        [DataMember]
        public int Layer { get; set; }
    }
}