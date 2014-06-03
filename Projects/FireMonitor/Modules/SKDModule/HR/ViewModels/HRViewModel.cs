using FiresecAPI.SKD;
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
		CardFilter CardFilter;
		AccessTemplateFilter AccessTemplateFilter;
		DocumentFilter DocumentFilter;

		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public DepartmentsViewModel DepartmentsViewModel { get; private set; }
		public PositionsViewModel PositionsViewModel { get; private set; }
		public AdditionalColumnTypesViewModel AdditionalColumnTypesViewModel { get; private set; }
		public CardsViewModel CardsViewModel { get; private set; }
		public AccessTemplatesViewModel AccessTemplatesViewModel { get; private set; }
		public OrganisationsViewModel OrganisationsViewModel { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; private set; }

		public HRViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);

			EmployeesViewModel = new EmployeesViewModel();
			DepartmentsViewModel = new DepartmentsViewModel();
			PositionsViewModel = new PositionsViewModel();
			AdditionalColumnTypesViewModel = new AdditionalColumnTypesViewModel();
			CardsViewModel = new CardsViewModel();
			AccessTemplatesViewModel = new AccessTemplatesViewModel();
			OrganisationsViewModel = new OrganisationsViewModel();
			DocumentsViewModel = new DocumentsViewModel();
			IsEmployeesSelected = true;

			var userUID = FiresecManager.CurrentUser.UID;
			Filter = new HRFilter() { UserUID = userUID };
			Filter.EmployeeFilter.UserUID = userUID;
			Filter.DepartmentFilter.UserUID = userUID;
			Filter.PositionFilter.UserUID = userUID;
			InitializeFilters();
			Initialize();
		}

		public void Initialize()
		{
			EmployeesViewModel.Initialize(EmployeeFilter);
			DepartmentsViewModel.Initialize(DepartmentFilter);
			PositionsViewModel.Initialize(PositionFilter);
			AdditionalColumnTypesViewModel.Initialize(AdditionalColumnTypeFilter);
			CardsViewModel.Initialize(CardFilter);
			AccessTemplatesViewModel.Initialize(AccessTemplateFilter);
			OrganisationsViewModel.Initialize();
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

		bool _isCardsSelected;
		public bool IsCardsSelected
		{
			get { return _isCardsSelected; }
			set
			{
				_isCardsSelected = value;
				OnPropertyChanged(() => IsCardsSelected);
			}
		}

		bool _isAccessTemplatesSelected;
		public bool IsAccessTemplatesSelected
		{
			get { return _isAccessTemplatesSelected; }
			set
			{
				_isAccessTemplatesSelected = value;
				OnPropertyChanged(() => IsAccessTemplatesSelected);
			}
		}

		bool _isOrganisationsSelected;
		public bool IsOrganisationsSelected
		{
			get { return _isOrganisationsSelected; }
			set
			{
				_isOrganisationsSelected = value;
				OnPropertyChanged(() => IsOrganisationsSelected);
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
				InitializeFilters();
				Initialize();
			}
		}

		void InitializeFilters()
		{
			EmployeeFilter = Filter.EmployeeFilter;
			DepartmentFilter = Filter.DepartmentFilter;
			PositionFilter = Filter.PositionFilter;
			AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
			CardFilter = Filter.CardFilter;
			AccessTemplateFilter = new AccessTemplateFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
			DocumentFilter = new DocumentFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
		}
	}
}