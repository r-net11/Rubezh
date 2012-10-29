using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XFiresecAPI
{
    [DataContract]
    public class LibraryXState
    {
        public LibraryXState()
        {
            XFrames = new List<LibraryXFrame>();
            XFrames.Add(new LibraryXFrame() { Id = 0 });
            Layer = 0;
        }

        [DataMember]
        public XStateClass XStateClass { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public List<LibraryXFrame> XFrames { get; set; }

        [DataMember]
        public int Layer { get; set; }
    }
}