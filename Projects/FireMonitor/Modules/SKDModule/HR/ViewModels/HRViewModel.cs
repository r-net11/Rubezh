using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class HRViewModel : ViewPartViewModel
	{
		SKDTabItems SKDTabItems;
		HRFilter Filter { get { return SKDTabItems.Filter; } set { SKDTabItems.Filter = value; } }
		EmployeeFilter EmployeeFilter;
		DepartmentFilter DepartmentFilter;
		PositionFilter PositionFilter;
		AdditionalColumnTypeFilter AdditionalColumnTypeFilter;
		CardFilter CardFilter;
		AccessTemplateFilter AccessTemplateFilter;
		PassCardTemplateFilter PassCardTemplateFilter;

		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public DepartmentsViewModel DepartmentsViewModel { get; private set; }
		public PositionsViewModel PositionsViewModel { get; private set; }
		public AdditionalColumnTypesViewModel AdditionalColumnTypesViewModel { get; private set; }
		public CardsViewModel CardsViewModel { get; private set; }
		public AccessTemplatesViewModel AccessTemplatesViewModel { get; private set; }
		public PassCardTemplatesViewModel PassCardTemplatesViewModel { get; private set; }
		public OrganisationsViewModel OrganisationsViewModel { get; private set; }

		public HRViewModel(SKDTabItems skdTabItems)
		{
			SKDTabItems = skdTabItems;
			EditFilterCommand = new RelayCommand(OnEditFilter, CanEditFilter);
			ChangeIsDeletedCommand = new RelayCommand(OnChangeIsDeleted, CanChangeIsDeleted);
			EmployeesViewModel = new EmployeesViewModel();
			DepartmentsViewModel = new DepartmentsViewModel();
			PositionsViewModel = new PositionsViewModel();
			AdditionalColumnTypesViewModel = new AdditionalColumnTypesViewModel();
			CardsViewModel = new CardsViewModel();
			AccessTemplatesViewModel = new AccessTemplatesViewModel();
			PassCardTemplatesViewModel = new PassCardTemplatesViewModel();
			OrganisationsViewModel = new OrganisationsViewModel();
			DepartmentFilter = new DepartmentFilter();
			PositionFilter = new PositionFilter();
			CardFilter = new CardFilter();
			if (CanSelectEmployees) 
				IsEmployeesSelected = true;
			else if (CanSelectDepartments) 
				IsDepartmentsSelected = true;
			else if (CanSelectPositions) 
				IsPositionsSelected = true;
			else if (CanSelectAdditionalColumns) 
				IsAdditionalColumnTypesSelected = true;
			else if (CanSelectCards) 
				IsCardsSelected = true;
			else if (CanSelectAccessTemplates) 
				IsAccessTemplatesSelected = true;
			else if (CanSelectPassCardTemplates) 
				IsPassCardTemplatesSelected = true;
			else if (CanSelectOrganisations) 
				IsOrganisationsSelected = true;
			PersonTypes = new ObservableCollection<PersonType>();
			if (ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View))
				PersonTypes.Add(PersonType.Employee);
			if (ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View))
				PersonTypes.Add(PersonType.Guest);
			_selectedPersonType = PersonTypes.FirstOrDefault();
			var user = ClientManager.CurrentUser;
			Filter = new HRFilter() { User = user };
			Filter.EmployeeFilter.User = user;
		}

		bool IsConnected
		{
			get { return ((SafeFiresecService)ClientManager.FiresecService).IsConnected; }
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

		bool _isPassCardTemplatesSelected;
		public bool IsPassCardTemplatesSelected
		{
			get { return _isPassCardTemplatesSelected; }
			set
			{
				_isPassCardTemplatesSelected = value;
				OnPropertyChanged(() => IsPassCardTemplatesSelected);
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

		public bool CanSelectEmployees
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View) || ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View); }
		}

		public bool CanSelectPositions
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Positions_View); }
		}

		public bool CanSelectDepartments
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Departments_View); }
		}

		public bool CanSelectAdditionalColumns
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AdditionalColumns_View); }
		}

		public bool CanSelectCards
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Cards_View); }
		}

		public bool CanSelectAccessTemplates
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AccessTemplates_View); }
		}

		public bool CanSelectPassCardTemplates
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_PassCards_View); }
		}

		public bool CanSelectOrganisations
		{
			get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Organisations_View); }
		}

		public ObservableCollection<PersonType> PersonTypes { get; private set; }

		PersonType _selectedPersonType;
		public PersonType SelectedPersonType
		{
			get { return _selectedPersonType; }
			set
			{
				_selectedPersonType = value;
				OnPropertyChanged(() => SelectedPersonType);
				InitializeEmployeeFilter();
				EmployeesViewModel.Initialize(EmployeeFilter);
			}
		}

		public bool IsWithDeleted
		{
			get { return Filter.LogicalDeletationType == LogicalDeletationType.All; }
			set
			{
				Filter.LogicalDeletationType = value ? LogicalDeletationType.All : LogicalDeletationType.Active;
				OnPropertyChanged(() => IsWithDeleted);
				SKDTabItems.Initialize();
			}
		}

		public bool CanSelectPersonType { get { return PersonTypes.Count == 2 && IsConnected; } }

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new HRFilterViewModel(Filter, IsEmployeesSelected || IsCardsSelected, SelectedPersonType == PersonType.Employee);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				OnPropertyChanged(() => IsWithDeleted);
				OnPropertyChanged(() => FilterImageSource);
				SKDTabItems.Initialize();
			}
		}
		bool CanEditFilter()
		{
			return IsConnected;
		}

		public RelayCommand ChangeIsDeletedCommand { get; private set; }
		void OnChangeIsDeleted()
		{
			IsWithDeleted = !IsWithDeleted;
		}
		bool CanChangeIsDeleted()
		{
			return IsConnected;
		}

		public string FilterImageSource { get { return Filter.EmployeeFilter.IsNotEmpty ? "archive" : "filter"; } }
		
		public void Initialize()
		{
			DepartmentFilter = new DepartmentFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				LogicalDeletationType = Filter.LogicalDeletationType 
			};
			PositionFilter = new PositionFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				LogicalDeletationType = Filter.LogicalDeletationType 
			};
			AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				LogicalDeletationType = Filter.LogicalDeletationType 
			};
			CardFilter = new CardFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				EmployeeFilter = Filter.EmployeeFilter, 
			};
			AccessTemplateFilter = new AccessTemplateFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				LogicalDeletationType = Filter.LogicalDeletationType 
			};
			PassCardTemplateFilter = new PassCardTemplateFilter() 
			{ 
				OrganisationUIDs = Filter.OrganisationUIDs, 
				LogicalDeletationType = Filter.LogicalDeletationType 
			};
			InitializeEmployeeFilter();
			DepartmentsViewModel.Initialize(DepartmentFilter);
			PositionsViewModel.Initialize(PositionFilter);
			AdditionalColumnTypesViewModel.Initialize(AdditionalColumnTypeFilter);
			CardsViewModel.Initialize(CardFilter);
			AccessTemplatesViewModel.Initialize(AccessTemplateFilter);
			PassCardTemplatesViewModel.Initialize(PassCardTemplateFilter);
			OrganisationsViewModel.Initialize(Filter.LogicalDeletationType);
			EmployeesViewModel.Initialize(EmployeeFilter);
        }

		void InitializeEmployeeFilter()
		{
			EmployeeFilter = Filter.EmployeeFilter;
			EmployeeFilter.UIDs = Filter.EmplooyeeUIDs;
			EmployeeFilter.PersonType = SelectedPersonType;
			EmployeeFilter.LogicalDeletationType = Filter.LogicalDeletationType;
		}

		public bool ShowFromJournal(Guid uid)
		{
			bool isFounded = false;
			if(EmployeesViewModel.ShowFromJournal(uid))
			{
				IsEmployeesSelected = true;
				isFounded = true;
			}
			if(DepartmentsViewModel.ShowFromJournal(uid))
			{
				IsDepartmentsSelected = true;
				isFounded = true;
			}
			if(PositionsViewModel.ShowFromJournal(uid))
			{
				IsPositionsSelected = true;
				isFounded = true;
			}
			if(AdditionalColumnTypesViewModel.ShowFromJournal(uid))
			{
				IsAdditionalColumnTypesSelected = true;
				isFounded = true;
			}
			if(AccessTemplatesViewModel.ShowFromJournal(uid))
			{
				IsAccessTemplatesSelected = true;
				isFounded = true;
			}
			if(PassCardTemplatesViewModel.ShowFromJournal(uid))
			{
				IsPassCardTemplatesSelected = true;
				isFounded = true;
			}
			if(OrganisationsViewModel.ShowFromJournal(uid))
			{
				IsOrganisationsSelected = true;
				isFounded = true;
			}
			if (isFounded)
				ApplicationService.Layout.Show(this);
			return isFounded;
		}
	}
}