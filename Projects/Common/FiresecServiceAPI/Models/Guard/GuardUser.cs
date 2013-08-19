using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class GuardUser
    {
        public GuardUser()
        {
			ZoneUIDs = new List<Guid>();
        	KeyTM = "00 00 00 00 00 00";
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool CanSetZone { get; set; }

        [DataMember]
        public bool CanUnSetZone { get; set; }

        [DataMember]
        public string FIO { get; set; }

        [DataMember]
        public string Function { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public Guid DeviceUID { get; set; }

        [DataMember]
        public string KeyTM { get; set; }

        [DataMember]
		public List<Guid> ZoneUIDs { get; set; }
    }
}
