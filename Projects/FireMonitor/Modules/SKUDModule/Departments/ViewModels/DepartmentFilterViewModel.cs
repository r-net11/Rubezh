using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DepartmentFilterViewModel : OrganizationFilterBaseViewModel<DepartmentFilter>
	{
		public DepartmentFilterViewModel(DepartmentFilter filter) : base(filter) { }
	}
}