using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class Employee
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string SecondName { get; set; }
        [DataMember]
        public string LastName { get; set; }
		[DataMember]
		public DateTime? Appointed { get; set; }
		[DataMember]
		public DateTime? Dismissed { get; set; }
        [DataMember]
        public Guid? PositionUid { get; set; }
        [DataMember]
        public Guid? DepartmentUid { get; set; }
        [DataMember]
        public Guid? ReplacementUid { get; set; }
        [DataMember]
        public Guid? ScheduleUid { get; set; }
        [DataMember]
        public List<Guid> AdditionalColumnUids { get; set; }
		[DataMember]
		public List<Guid> CardUids { get; set; } 
	}
}
