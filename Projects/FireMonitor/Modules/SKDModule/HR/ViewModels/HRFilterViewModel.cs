using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class HRFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public EmployeeFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public bool ShowEmployeeFilter { get; private set; }

		public HRFilterViewModel(HRFilter filter, bool showEmployeeFilter)
			: base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
			ShowEmployeeFilter = showEmployeeFilter;
			InitializeFilter(filter);
		}

		void InitializeFilter(HRFilter filter)
		{
			EmployeeFilterViewModel = new EmployeeFilterViewModel(filter.EmployeeFilter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel(filter.EmployeeFilter);
			PositionsFilterViewModel = new PositionsFilterViewModel(filter.EmployeeFilter);
			IsWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeeFilterViewModel.Save();
			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs; 
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs.ToList();
			Filter.LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			return true;
		}

		bool _isWithDeleted;
		public bool IsWithDeleted
		{
			get { return _isWithDeleted; }
			set
			{
				_isWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new HRFilter();
			InitializeFilter(Filter);
		}
	}
}