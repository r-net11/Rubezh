using Localization.SKD.ViewModels;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class ApplyAccessTemplateToUserGroupFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public ApplyAccessTemplateToUserGroupFilterViewModel(HRFilter filter, bool showPositions)
			: base(filter, true, new CurrentUserFilteredOrganisationsProvider(filter.OrganisationUIDs))
		{
			ShowPositions = showPositions;
			InitializeFilter(filter);
		}

		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public AccessTemplatesFilterViewModel AccessTemplatesFilterViewModel { get; private set; }
		public ApplyAccessTemplateToUserGroupEmployeesFilterViewModel EmployeesFilterViewModel { get; private set; }
		public bool ShowPositions { get; private set; }

		public string EmployeesFilterTitle
		{
			get 
			{
				return Filter.EmployeeFilter.PersonType == PersonType.Employee
					? CommonViewModels.Employees
					: CommonViewModels.Visitors;
			}
		}

		void InitializeFilter(HRFilter filter)
		{
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel();
			DepartmentsFilterViewModel.Initialize(filter.EmployeeFilter.DepartmentUIDs, filter.OrganisationUIDs, filter.LogicalDeletationType);
			PositionsFilterViewModel = new PositionsFilterViewModel();
			PositionsFilterViewModel.Initialize(filter.EmployeeFilter.PositionUIDs, filter.OrganisationUIDs, filter.LogicalDeletationType);
			AccessTemplatesFilterViewModel = new AccessTemplatesFilterViewModel();
			AccessTemplatesFilterViewModel.Initialize(filter.OrganisationUIDs, filter.LogicalDeletationType);
			EmployeesFilterViewModel = new ApplyAccessTemplateToUserGroupEmployeesFilterViewModel();
			EmployeesFilterViewModel.Initialize(filter.EmployeeFilter);
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeesFilterViewModel.Filter;
			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs; 
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs;
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs;
			Filter.LogicalDeletationType = LogicalDeletationType.Active;
			return true;
		}
	}
}