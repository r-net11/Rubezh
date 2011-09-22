using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class User
    {
        public User()
        {
            Groups = new List<string>();
            Permissions = new List<string>();
            RemovedPermissions = new List<string>();
            Permisions = new List<PermissionType>();
            RemovedPermisions = new List<PermissionType>();
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public bool IsBuiltIn { get; set; }

        [DataMember]
        public List<string> Groups { get; set; }

        [DataMember]
        public List<PermissionType> Permisions { get; set; }

        [DataMember]
        public List<PermissionType> RemovedPermisions { get; set; }

        [DataMember]
        public List<string> Permissions { get; set; }

        [DataMember]
        public List<string> RemovedPermissions { get; set; }
    }
}