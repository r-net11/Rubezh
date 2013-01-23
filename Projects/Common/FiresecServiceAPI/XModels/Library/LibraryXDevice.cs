using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class LibraryXDevice
    {
        public LibraryXDevice()
        {
            XStates = new List<LibraryXState>();
            var libraryXState = new LibraryXState()
            {
                XStateClass = XStateClass.No,
            };
            XStates.Add(libraryXState);
        }

        public XDriver Driver { get; set; }

        [DataMember]
        public Guid XDriverId { get; set; }

        [DataMember]
        public List<LibraryXState> XStates { get; set; }
    }
}