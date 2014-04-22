using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DepartmentFilterViewModel : OrganisationFilterBaseViewModel<DepartmentFilter>
	{
		public DepartmentFilterViewModel(DepartmentFilter filter) : base(filter) { }
	}
}