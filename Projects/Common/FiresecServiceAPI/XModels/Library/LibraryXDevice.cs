using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XFiresecAPI
{
    public class LibraryXDevice
    {
        public LibraryXDevice()
        {
            XStates = new List<LibraryXState>();
            var libraryXState = new LibraryXState()
            {
                XStateType = XStateType.No,
            };
            XStates.Add(libraryXState);
        }

        public XDriver XDriver { get; set; }

        [DataMember]
        public Guid XDriverId { get; set; }

        [DataMember]
        public List<LibraryXState> XStates { get; set; }
    }
}
