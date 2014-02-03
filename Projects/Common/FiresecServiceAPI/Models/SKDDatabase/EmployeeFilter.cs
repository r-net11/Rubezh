using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class EmployeeFilter
    {
        [DataMember]
        public List<Guid> Uids { get; set; }
        [DataMember]
        public List<Guid> PositionUids { get; set; }
        [DataMember]
        public List<Guid> DepartmentUids { get; set; }
        [DataMember]
        public DateTimePeriod Appointed { get; set; }
        [DataMember]
        public DateTimePeriod Dismissed { get; set; }

        public bool HasUids
        {
            get { return Uids.Count > 0; }
        }
        
        public bool HasPositions
        {
            get{ return PositionUids.Count > 0; }
        }

        public bool HasDepartments
        {
            get { return DepartmentUids.Count > 0; }
        }

        public EmployeeFilter()
        {
            Uids = new List<Guid>();
            PositionUids = new List<Guid>();
            DepartmentUids = new List<Guid>();
        }
    }
}
