﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;

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
			EditFilterCommand = new RelayCommand(OnEditFilter);
			ChangeIsDeletedCommand = new RelayCommand(OnChangeIsDeleted);

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
			if (CanSelectEmployees) IsEmployeesSelected = true;
			else if (CanSelectDepartments) IsDepartmentsSelected = true;
			else if (CanSelectPositions) IsPositionsSelected = true;
			else if (CanSelectAdditionalColumns) IsAdditionalColumnTypesSelected = true;
			else if (CanSelectCards) IsCardsSelected = true;
			else if (CanSelectAccessTemplates) IsAccessTemplatesSelected = true;
			else if (CanSelectPassCardTemplates) IsPassCardTemplatesSelected = true;
			else if (CanSelectOrganisations) IsOrganisationsSelected = true;
				

			PersonTypes = new ObservableCollection<PersonType>();
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View))
				PersonTypes.Add(PersonType.Employee);
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View))
				PersonTypes.Add(PersonType.Guest);
			_selectedPersonType = PersonTypes.FirstOrDefault();
			CanSelectPersonType = PersonTypes.Count == 2;

			var userUID = FiresecManager.CurrentUser.UID;
			Filter = new HRFilter() { UserUID = userUID };
			Filter.EmployeeFilter.UserUID = userUID;
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
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View) || FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View); }
		}

		public bool CanSelectPositions
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Positions_View); }
		}

		public bool CanSelectDepartments
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Departments_View); }
		}

		public bool CanSelectAdditionalColumns
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AdditionalColumns_View); }
		}

		public bool CanSelectCards
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Cards_View); }
		}

		public bool CanSelectAccessTemplates
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AccessTemplates_View); }
		}

		public bool CanSelectPassCardTemplates
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_PassCards_View); }
		}

		public bool CanSelectOrganisations
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Organisations_View); }
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

		public bool CanSelectPersonType { get; private set; }

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
		public RelayCommand ChangeIsDeletedCommand { get; private set; }
		void OnChangeIsDeleted()
		{
			IsWithDeleted = !IsWithDeleted;
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
				ClientUID = CardsViewModel.DbCallbackResultUID, 
				IsLoad = true 
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
			EmployeeFilter.ClientUID = EmployeesViewModel.DbCallbackResultUID; 
			EmployeeFilter.IsLoad = true;
        }

		void BeginGetAsync()
		{
			var hrViewModel = new FiresecAPI.SKD.HRFilter
			{
				AccessTemplateFilter = AccessTemplateFilter,
				AdditionalColumnTypeFilter = AdditionalColumnTypeFilter,
				CardFilter = CardFilter,
				DepartmentFilter = DepartmentFilter,
				EmployeeFilter = EmployeeFilter,
				PassCardTemplateFilter = PassCardTemplateFilter,
				PositionFilter = PositionFilter
			};
			FiresecManager.FiresecService.BeginGetAsync(hrViewModel);
		}
	}
}