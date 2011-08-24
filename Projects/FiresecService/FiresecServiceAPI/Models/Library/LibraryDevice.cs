using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryDevice
    {
        [DataMember]
        public string DriverId { get; set; }

        [DataMember]
        public List<LibraryState> States { get; set; }
    }
}