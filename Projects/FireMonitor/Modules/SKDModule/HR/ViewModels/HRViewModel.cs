using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using SKDModule.PassCardDesigner.ViewModels;

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
		PassCardTemplateFilter PassCardTemplateFilter;

		List<CardReportFilter> CardReportFilters; 

		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public DepartmentsViewModel DepartmentsViewModel { get; private set; }
		public PositionsViewModel PositionsViewModel { get; private set; }
		public AdditionalColumnTypesViewModel AdditionalColumnTypesViewModel { get; private set; }
		public CardsViewModel CardsViewModel { get; private set; }
		public AccessTemplatesViewModel AccessTemplatesViewModel { get; private set; }
		public PassCardTemplatesViewModel PassCardTemplatesViewModel { get; private set; }
		public OrganisationsViewModel OrganisationsViewModel { get; private set; }

		public HRViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			ShowCardReportFilterDetailsCommand = new RelayCommand(OnShowCardReportFilterDetails);

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
			IsEmployeesSelected = true;

			CardReportFilters = new List<CardReportFilter>();
			CardReportFilters.Add(new CardReportFilter { Name = "По умолчанию" });
			CardReportFilters.Add(new CardReportFilter { Name = "Фильтр 1" });
			CardReportFilters.Add(new CardReportFilter { Name = "Фильтр 2" });

			PersonTypes = new ObservableCollection<PersonType>();
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees))
				PersonTypes.Add(PersonType.Employee);
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests))
				PersonTypes.Add(PersonType.Guest);
			_selectedPersonType = PersonTypes.FirstOrDefault();
			CanSelectPersonType = PersonTypes.Count == 2;

			var userUID = FiresecManager.CurrentUser.UID;
			Filter = new HRFilter() { UserUID = userUID };
			Filter.EmployeeFilter.UserUID = userUID;
			InitializeFilters();
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
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

		public bool CanSelectHR
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_HR); }
		}
		public bool CanSelectOrganisations
		{
			get { return FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Organisations); }
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
			}
		}

		public bool IsWithDeleted
		{
			get { return Filter.LogicalDeletationType == LogicalDeletationType.All; }
			set
			{
				Filter.LogicalDeletationType = value ? LogicalDeletationType.All : LogicalDeletationType.Active;
				OnPropertyChanged(() => IsWithDeleted);
				InitializeFilters();
			}
		}

		public bool CanSelectPersonType { get; private set; }

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new HRFilterViewModel(Filter, IsEmployeesSelected);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				OnPropertyChanged(() => IsWithDeleted);
				InitializeFilters();
			}
		}

		void InitializeFilters()
		{
			DepartmentFilter = new DepartmentFilter() { OrganisationUIDs = Filter.OrganisationUIDs, LogicalDeletationType = Filter.LogicalDeletationType, EmployeeUIDs = Filter.EmplooyeeUIDs };
			PositionFilter = new PositionFilter() { OrganisationUIDs = Filter.OrganisationUIDs, LogicalDeletationType = Filter.LogicalDeletationType, EmployeeUIDs = Filter.EmplooyeeUIDs };
			AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter() { OrganisationUIDs = Filter.OrganisationUIDs, LogicalDeletationType = Filter.LogicalDeletationType };
			CardFilter = new CardFilter() { OrganisationUIDs = Filter.OrganisationUIDs };
			AccessTemplateFilter = new AccessTemplateFilter() { OrganisationUIDs = Filter.OrganisationUIDs, LogicalDeletationType = Filter.LogicalDeletationType };
			PassCardTemplateFilter = new PassCardTemplateFilter() { OrganisationUIDs = Filter.OrganisationUIDs, LogicalDeletationType = Filter.LogicalDeletationType };

			DepartmentsViewModel.Initialize(DepartmentFilter);
			PositionsViewModel.Initialize(PositionFilter);
			AdditionalColumnTypesViewModel.Initialize(AdditionalColumnTypeFilter);
			CardsViewModel.Initialize(CardFilter);
			AccessTemplatesViewModel.Initialize(AccessTemplateFilter);
			PassCardTemplatesViewModel.Initialize(PassCardTemplateFilter);
			OrganisationsViewModel.Initialize(Filter.LogicalDeletationType);

			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Publish(Filter.LogicalDeletationType);
			ServiceFactory.Events.GetEvent<UpdateFilterEvent>().Publish(Filter);

			InitializeEmployeeFilter();
		}

		void InitializeEmployeeFilter()
		{
			EmployeeFilter = Filter.EmployeeFilter;
			EmployeeFilter.UIDs = Filter.EmplooyeeUIDs;
			EmployeeFilter.PersonType = SelectedPersonType;
			EmployeeFilter.LogicalDeletationType = Filter.LogicalDeletationType;
			EmployeesViewModel.Initialize(EmployeeFilter);
		}

		public RelayCommand ShowCardReportFilterDetailsCommand { get; private set; }
		void OnShowCardReportFilterDetails()
		{
			var cardReportFilterDetailsViewModel = new CardReportFilterDetailsViewModel(CardReportFilters);
			if (DialogService.ShowModalWindow(cardReportFilterDetailsViewModel))
			{
				var cardReportItemsResult = FiresecManager.FiresecService.GetCardReport(cardReportFilterDetailsViewModel.SelectedCardReportFilter);
				if (cardReportItemsResult.HasError)
					return;
				var cardReportItems = cardReportItemsResult.Result;
				foreach (var item in cardReportItems)
				{
					Trace.WriteLine(item.CardType + " " + item.Number + " " + item.Organisation + " " + item.Department + " " + item.Position + " " + item.Employee + " " + item.EndDate);
				}
			}
		}
	}
}