using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
    public class Holiday
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public HolidayType Type { get; set; }
        [DataMember]
        public DateTime? Date { get; set; }
        [DataMember]
        public DateTime? TransferDate { get; set; }
        [DataMember]
        public int? Reduction { get; set; }
	}

    [DataContract]
    public enum HolidayType
	{
		Holiday,
		Reduced,
		Transferred,
		Working
	}
}
