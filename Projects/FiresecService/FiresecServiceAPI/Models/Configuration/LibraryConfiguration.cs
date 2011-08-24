using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryConfiguration
    {
        public LibraryConfiguration()
        {
            Devices = new List<LibraryDevice>();
        }

        [DataMember]
        public List<LibraryDevice> Devices { get; set; }
    }
}