using System.Runtime.Serialization;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class HRFilter : OrganisationFilterBase
	{
		public HRFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			DepartmentFilter = new DepartmentFilter();
			PositionFilter = new PositionFilter();
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		[DataMember]
		public DepartmentFilter DepartmentFilter { get; set; }

		[DataMember]
		public PositionFilter PositionFilter { get; set; }
	}
}