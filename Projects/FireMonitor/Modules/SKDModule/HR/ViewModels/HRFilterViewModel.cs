using System.Threading.Tasks;
using Localization.SKD.ViewModels;
using SKDModule.Common;
using StrazhAPI.SKD;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class HRFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public EmployeesFilterViewModel EmployeesFilterViewModel { get; private set; }
		public bool ShowEmployeeFilter { get; private set; }
		public bool IsShowPositions { get; private set; }

		public string EmployeesFilterHeaderTitle
		{
			get
			{
				return Filter.EmployeeFilter.PersonType == PersonType.Employee
					? CommonViewModels.Employees
					: CommonViewModels.Visitors;
			}
		}

		public HRFilterViewModel(HRFilter filter, bool showEmployeeFilter, bool isShowPositions)
			: base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
			ShowEmployeeFilter = showEmployeeFilter;
			IsShowPositions = isShowPositions;
			InitializeFilter(filter);
		}

		private void InitializeFilter(HRFilter filter)
		{
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel();
			DepartmentsFilterViewModel.Initialize(filter.EmployeeFilter.DepartmentUIDs, filter.LogicalDeletationType);
			PositionsFilterViewModel = new PositionsFilterViewModel();
			PositionsFilterViewModel.Initialize(filter.EmployeeFilter.PositionUIDs, filter.LogicalDeletationType);
			EmployeesFilterViewModel = new EmployeesFilterViewModel();
			EmployeesFilterViewModel.Initialize(filter.EmployeeFilter);
			IsWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeesFilterViewModel.Filter;
			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs;
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs;
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs;
			Filter.LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			return true;
		}

		private bool _isWithDeleted;
		public bool IsWithDeleted
		{
			get { return _isWithDeleted; }
			set
			{
				_isWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
				Filter.LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
				Filter.EmployeeFilter.LogicalDeletationType = IsWithDeleted
					? LogicalDeletationType.All
					: LogicalDeletationType.Active;
				UpdateUI();
			}
		}

		private void UpdateUI()
		{
			Task.Factory.StartNew(() => EmployeesFilterViewModel.Initialize(Filter.EmployeeFilter));
			Task.Factory.StartNew(() => PositionsFilterViewModel.Initialize(Filter.EmployeeFilter.PositionUIDs, Filter.LogicalDeletationType));
			Task.Factory.StartNew(() => DepartmentsFilterViewModel.Initialize(Filter.EmployeeFilter.DepartmentUIDs, Filter.LogicalDeletationType));
			Task.Factory.StartNew(() => UpdateOrganisations(IsWithDeleted ? new CurrentUserDeletedOrganisationsProvider() : null));
		}

		public RelayCommand ResetCommand { get; private set; }
		private void OnReset()
		{
			Filter = new HRFilter();
			InitializeFilter(Filter);
		}
	}
}