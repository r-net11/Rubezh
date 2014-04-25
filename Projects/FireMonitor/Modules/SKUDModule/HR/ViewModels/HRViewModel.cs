using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HRViewModel : ViewPartViewModel
	{
		HRFilter Filter;
		EmployeeFilter EmployeeFilter;
		DepartmentFilter DepartmentFilter;
		PositionFilter PositionFilter;
		AdditionalColumnTypeFilter AdditionalColumnTypeFilter;
		DocumentFilter DocumentFilter;

		public HRViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new HRFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };

			EmployeesViewModel = new EmployeesViewModel();
			DepartmentsViewModel = new DepartmentsViewModel();
			PositionsViewModel = new PositionsViewModel();
			AdditionalColumnTypesViewModel = new AdditionalColumnTypesViewModel();
			DocumentsViewModel = new DocumentsViewModel();


			EmployeeFilter = new EmployeeFilter();
			if (FiresecManager.CurrentUser.IsGuestsAllowed)
				EmployeeFilter.PersonType = PersonType.Guest;
			else
				EmployeeFilter.PersonType = PersonType.Employee;
			
			var organisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs;
			if (organisationUIDs.IsNotNullOrEmpty())
				EmployeeFilter.OrganisationUIDs = organisationUIDs;

			DepartmentFilter = new DepartmentFilter();
			PositionFilter = new PositionFilter();
			AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			DocumentFilter = new DocumentFilter();

			Initialize();
		}

		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public DepartmentsViewModel DepartmentsViewModel { get; private set; }
		public PositionsViewModel PositionsViewModel { get; private set; }
		public AdditionalColumnTypesViewModel AdditionalColumnTypesViewModel { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; private set; }

		public void Initialize()
		{
			EmployeesViewModel.Initialize(EmployeeFilter);
			DepartmentsViewModel.Initialize(DepartmentFilter);
			PositionsViewModel.Initialize(PositionFilter);
			AdditionalColumnTypesViewModel.Initialize(AdditionalColumnTypeFilter);
			DocumentsViewModel.Initialize(DocumentFilter);
		}

		bool _isEmployeesSelected;
		public bool IsEmployeesSelected
		{
			get { return _isEmployeesSelected; }
			set
			{
				_isEmployeesSelected = value;
				OnPropertyChanged(() => IsEmployeesSelected);
			}
		}

		bool _isDepartmentsSelected;
		public bool IsDepartmentsSelected
		{
			get { return _isDepartmentsSelected; }
			set
			{
				_isDepartmentsSelected = value;
				OnPropertyChanged(() => IsDepartmentsSelected);
			}
		}

		bool _isPositionsSelected;
		public bool IsPositionsSelected
		{
			get { return _isPositionsSelected; }
			set
			{
				_isPositionsSelected = value;
				OnPropertyChanged(() => IsPositionsSelected);
			}
		}

		bool _isAdditionalColumnTypesSelected;
		public bool IsAdditionalColumnTypesSelected
		{
			get { return _isAdditionalColumnTypesSelected; }
			set
			{
				_isAdditionalColumnTypesSelected = value;
				OnPropertyChanged(() => IsAdditionalColumnTypesSelected);
			}
		}

		bool _isDocumentsSelected;
		public bool IsDocumentsSelected
		{
			get { return _isDocumentsSelected; }
			set
			{
				_isDocumentsSelected = value;
				OnPropertyChanged(() => IsDocumentsSelected);
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new HRFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;

				EmployeeFilter = new EmployeeFilter();
				EmployeeFilter.PersonType = Filter.PersonType;
				EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs;
				
				DepartmentFilter = new DepartmentFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
				PositionFilter = new PositionFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
				AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
				DocumentFilter = new DocumentFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
				Initialize();
			}
		}
	}
}