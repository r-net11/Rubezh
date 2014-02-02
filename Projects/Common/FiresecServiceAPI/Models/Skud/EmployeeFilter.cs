using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
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

    [DataContract]
    public class DateTimePeriod
    {
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
