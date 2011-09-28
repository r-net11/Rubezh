using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryDevice
    {
        [DataMember]
        public Guid DriverId { get; set; }

        [DataMember]
        public List<LibraryState> States { get; set; }
    }
}