using RubezhAPI.SKD;
using Infrastructure.Common;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class HRFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public EmployeesFilterViewModel EmployeesFilterViewModel { get; private set; }
		public bool ShowEmployeeFilter { get; private set; }
		public bool IsShowPositions { get; private set; }

		public HRFilterViewModel(HRFilter filter, bool showEmployeeFilter, bool isShowPositions)
			: base(filter)
		{
			ShowEmployeeFilter = showEmployeeFilter;
			IsShowPositions = isShowPositions;
		}

		public override void Initialize(HRFilter filter)
		{
			base.Initialize(filter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel();
			DepartmentsFilterViewModel.Initialize(filter.EmployeeFilter.DepartmentUIDs, filter.LogicalDeletationType);
			PositionsFilterViewModel = new PositionsFilterViewModel();
			PositionsFilterViewModel.Initialize(filter.EmployeeFilter.PositionUIDs, filter.LogicalDeletationType);
			EmployeesFilterViewModel = new EmployeesFilterViewModel();
			EmployeesFilterViewModel.Initialize(filter.EmployeeFilter);
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeesFilterViewModel.Filter;
			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs; 
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs;
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs;
			Filter.LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			EmployeesFilterViewModel.Unsubscribe();
			PositionsFilterViewModel.Unsubscribe();
			DepartmentsFilterViewModel.Unsubscribe();
			return true;
		}

		protected override List<IHRFilterTab> HRFilterTabs
		{
			get { return new List<IHRFilterTab> { DepartmentsFilterViewModel, EmployeesFilterViewModel, PositionsFilterViewModel }; }
		}
	}
}