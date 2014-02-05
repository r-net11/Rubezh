using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class DepartmentFilter
    {
        [DataMember]
		public List<Guid> Uids { get; set; }

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
