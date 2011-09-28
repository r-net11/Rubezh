using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class SecurityConfiguration
    {
        public SecurityConfiguration()
        {
            Users = new List<User>();
            UserRoles = new List<UserRole>();
        }

        [DataMember]
        public List<User> Users { get; set; }

        [DataMember]
        public List<UserRole> UserRoles { get; set; }
    }
}