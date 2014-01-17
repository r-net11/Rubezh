using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
    [DataContract]
    public class DepartmentFilter
    {
        [DataMember]
        public List<Guid> Uids;

        public bool HasUids
        {
            get { return Uids.Count > 0; }
        }
        
        public DepartmentFilter()
        {
            Uids = new List<Guid>();
        }
    }
}
