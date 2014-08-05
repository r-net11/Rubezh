using System.Linq;
using Infrastructure.Common;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class HRFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public EmployeeFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }

		public HRFilterViewModel(HRFilter filter)
			: base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
			InitializeFilter(filter);
		}

		void InitializeFilter(HRFilter filter)
		{
			EmployeeFilterViewModel = new EmployeeFilterViewModel(filter.EmployeeFilter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel(filter.EmployeeFilter);
			PositionsFilterViewModel = new PositionsFilterViewModel(filter.EmployeeFilter);
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeeFilterViewModel.Save();
			Filter.CardFilter = new CardFilter();

			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs.ToList();

			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs; 
			Filter.DepartmentFilter.OrganisationUIDs = Filter.OrganisationUIDs;
			Filter.PositionFilter.OrganisationUIDs = Filter.OrganisationUIDs;

			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new HRFilter();
			InitializeFilter(Filter);
		}
	}
}