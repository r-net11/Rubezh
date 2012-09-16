using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryDevice
    {
        public LibraryDevice()
        {
            States = new List<LibraryState>();
            var libraryState = new LibraryState()
            {
                StateType = StateType.No,
            };
            States.Add(libraryState);
        }

        public Driver Driver { get; set; }

        [DataMember]
        public Guid DriverId { get; set; }

        [DataMember]
        public List<LibraryState> States { get; set; }
    }
}