using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class EmployeeReplacement
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public DateTime? BeginDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public Department Department { get; set; }
        [DataMember]
        public Schedule Schedule { get; set; }
	}
}
