using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Runtime.Serialization;

namespace SKDModule.ViewModels
{
	public class HRFilter : OrganizationFilterBase
	{
		public HRFilter()
			: base()
		{
			PositionUIDs = new List<Guid>();
			DepartmentUIDs = new List<Guid>();
			Appointed = new DateTimePeriod();
			Dismissed = new DateTimePeriod();
			PersonType = PersonType.Employee;
		}

		[DataMember]
		public List<Guid> PositionUIDs { get; set; }

		[DataMember]
		public List<Guid> DepartmentUIDs { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public DateTimePeriod Appointed { get; set; }

		[DataMember]
		public DateTimePeriod Dismissed { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }
	}
}