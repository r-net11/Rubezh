using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using CardReportFilter = Infrastructure.Common.SKDReports.Filters.CardReportFilter;

namespace SKDModule.ViewModels
{
	public class CardReportFilterDetailsViewModel: SaveCancelDialogViewModel
	{
		public CardReportFilterDetailsViewModel(List<CardReportFilter> cardReportFilters)
		{
			Title = "Настройки отчёта по пропускам";
			SelectShowAdditionalCommand = new RelayCommand(OnSelectShowAdditional);
			SelectEndDateCommand = new RelayCommand(OnSelectEndDate);
			SaveFilterCommand = new RelayCommand(OnSaveFilter);
			RemoveFilterCommand = new RelayCommand(OnRemoveFilter);
			CardReportFilters = new ObservableCollection<CardReportFilter>(cardReportFilters);

			EndDateTypes = new List<EndDateType>();
			foreach (EndDateType endDateType in Enum.GetValues(typeof(EndDateType)))
			{
				EndDateTypes.Add(endDateType);
			}
			SelectedEndDateType = EndDateType.Arbitrary;

			CardSortTypes = new List<CardSortType>();
			foreach (CardSortType CardSortType in Enum.GetValues(typeof(CardSortType)))
			{
				CardSortTypes.Add(CardSortType);
			}
			CardTypesViewModel = new CardTypesViewModel();
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel();
			PositionsFilterViewModel = new PositionsFilterViewModel();
			EmployeesFilterViewModel = new EmployeesFilterViewModel();
			OrganisationsFilterViewModel = new OrganisationsFilterViewModel();
			SelectedCardReportFilter = CardReportFilters.FirstOrDefault();
		}

		void CopyProperties()
		{
			CardTypesViewModel.Update(SelectedCardReportFilter);
			EndDate = SelectedCardReportFilter.CardFilter.EndDate;
			IsWithEndDate = SelectedCardReportFilter.CardFilter.IsWithEndDate;
			SelectedCardSortType = CardSortTypes.FirstOrDefault(x => x == SelectedCardReportFilter.CardSortType);
			IsSortAsc = SelectedCardReportFilter.IsSortAsc;
			IsShowUser = SelectedCardReportFilter.IsShowUser;
			IsShowDate = SelectedCardReportFilter.IsShowDate;
			IsShowPeriod = SelectedCardReportFilter.IsShowPeriod;
			IsShowNameInHeader = SelectedCardReportFilter.IsShowNameInHeader;
			IsShowName = SelectedCardReportFilter.IsShowName;
			_IsWithDeleted = SelectedCardReportFilter.EmployeeFilter.LogicalDeletationType == LogicalDeletationType.All;
			OnPropertyChanged(() => IsWithDeleted);
			_IsGuests = SelectedCardReportFilter.EmployeeFilter.PersonType == PersonType.Guest;
			OnPropertyChanged(() => IsGuests);
			OnPropertyChanged(() => EmployeeTabItemHeader);
			switch (SelectedCardReportFilter.CardFilter.DeactivationType)
			{
				case LogicalDeletationType.Active:
					IsWithActive = true;
					IsWithBlocked = false;
					break;
				case LogicalDeletationType.Deleted:
					IsWithActive = false;
					IsWithBlocked = true;
					break;
				case LogicalDeletationType.All:
					IsWithActive = true;
					IsWithBlocked = true;
					break;
				default:
					break;
			}
			DepartmentsFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter.DepartmentUIDs, DeletationType);
			PositionsFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter.PositionUIDs, DeletationType);
			EmployeesFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter, DeletationType, CurrentPersonType);
			OrganisationsFilterViewModel.Initialize(SelectedCardReportFilter.CardFilter.OrganisationUIDs);
		}

		void CopyPropertiesBack()
		{
			SelectedCardReportFilter.CardFilter.CardTypes = CardTypesViewModel.GetCheckedCardTypes();
			SelectedCardReportFilter.CardFilter.EndDate = EndDate;
			SelectedCardReportFilter.CardFilter.IsWithEndDate = IsWithEndDate;
			SelectedCardReportFilter.CardSortType = SelectedCardSortType;
			SelectedCardReportFilter.IsSortAsc = IsSortAsc;
			SelectedCardReportFilter.IsShowName = IsShowName;
			SelectedCardReportFilter.IsShowNameInHeader = IsShowNameInHeader;
			SelectedCardReportFilter.IsShowPeriod = IsShowPeriod;
			SelectedCardReportFilter.IsShowDate = IsShowDate;
			SelectedCardReportFilter.IsShowUser = IsShowUser;
			SelectedCardReportFilter.EmployeeFilter.LogicalDeletationType = DeletationType;
			SelectedCardReportFilter.EmployeeFilter.PersonType = CurrentPersonType;
			if (IsWithActive && IsWithBlocked)
				SelectedCardReportFilter.CardFilter.DeactivationType = LogicalDeletationType.All;
			else if(IsWithActive)
				SelectedCardReportFilter.CardFilter.DeactivationType = LogicalDeletationType.Active;
			else if (IsWithBlocked)
				SelectedCardReportFilter.CardFilter.DeactivationType = LogicalDeletationType.Deleted;
			else
				SelectedCardReportFilter.CardFilter.DeactivationType = LogicalDeletationType.All;
			if (EmployeesFilterViewModel.IsSelection)
				SelectedCardReportFilter.EmployeeFilter.UIDs = EmployeesFilterViewModel.UIDs;
			else
			{
				SelectedCardReportFilter.EmployeeFilter.FirstName = EmployeesFilterViewModel.FirstName;
				SelectedCardReportFilter.EmployeeFilter.SecondName = EmployeesFilterViewModel.SecondName;
				SelectedCardReportFilter.EmployeeFilter.LastName = EmployeesFilterViewModel.LastName;
			}
			SelectedCardReportFilter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs;
			SelectedCardReportFilter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs;
			SelectedCardReportFilter.CardFilter.OrganisationUIDs = OrganisationsFilterViewModel.UIDs;
		}

		public ObservableCollection<CardReportFilter> CardReportFilters { get; private set; }
		public CardTypesViewModel CardTypesViewModel { get; private set; }
		public List<EndDateType> EndDateTypes { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public EmployeesFilterViewModel EmployeesFilterViewModel { get; private set; }
		public OrganisationsFilterViewModel OrganisationsFilterViewModel { get; private set; }
		
		CardReportFilter _SelectedCardReportFilter;
		public CardReportFilter SelectedCardReportFilter
		{
			get { return _SelectedCardReportFilter; }
			set
			{
				if (value == null)
					return;
				_SelectedCardReportFilter = value;
				OnPropertyChanged(() => SelectedCardReportFilter);
				CopyProperties();
			}
		}

		EndDateType _SelectedEndDateType;
		public EndDateType SelectedEndDateType
		{
			get { return _SelectedEndDateType; }
			set
			{
				_SelectedEndDateType = value;
				OnPropertyChanged(() => SelectedEndDateType);
				if (_SelectedEndDateType == EndDateType.Arbitrary)
					return;
				var minusDate = new TimeSpan();
				switch(_SelectedEndDateType)
				{
					case EndDateType.Day:
						minusDate = new TimeSpan(1, 0, 0, 0);
						break;
					case EndDateType.Week:
						minusDate = new TimeSpan(7, 0, 0, 0);
						break;
					case EndDateType.Month:
						minusDate = new TimeSpan(30, 0, 0, 0);
						break;
				}
				EndDate = DateTime.Now.Date.Subtract(minusDate);
			}
		}

		public List<CardSortType> CardSortTypes { get; private set; }

		CardSortType _SelectedCardSortType;
		public CardSortType SelectedCardSortType
		{
			get { return _SelectedCardSortType; }
			set
			{
				_SelectedCardSortType = value;
				OnPropertyChanged(() => SelectedCardSortType);
			}
		}

		bool _isShowAdditional;
		public bool IsShowAdditional
		{
			get { return _isShowAdditional; }
			set
			{
				_isShowAdditional = value;
				OnPropertyChanged(() => IsShowAdditional);
				OnPropertyChanged(() => SelectShowAdditionalString);
			}
		}

		bool _isWithActive;
		public bool IsWithActive
		{
			get { return _isWithActive; }
			set
			{
				_isWithActive = value;
				OnPropertyChanged(() => IsWithActive);
				CardTypesViewModel.CanSelect = value;
			}
		}

		bool _IsWithEndDate;
		public bool IsWithEndDate
		{
			get { return _IsWithEndDate; }
			set
			{
				_IsWithEndDate = value;
				OnPropertyChanged(() => IsWithEndDate);
			}
		}

		bool _IsSortAsc;
		public bool IsSortAsc
		{
			get { return _IsSortAsc; }
			set
			{
				_IsSortAsc = value;
				OnPropertyChanged(() => IsSortAsc);
			}
		}

		bool _isWithBlocked;
		public bool IsWithBlocked
		{
			get { return _isWithBlocked; }
			set
			{
				_isWithBlocked = value;
				OnPropertyChanged(() => IsWithBlocked);
			}
		}

		bool _IsShowName;
		public bool IsShowName
		{
			get { return _IsShowName; }
			set
			{
				_IsShowName = value;
				OnPropertyChanged(() => IsShowName);
			}
		}

		bool _IsShowNameInHeader;
		public bool IsShowNameInHeader
		{
			get { return _IsShowNameInHeader; }
			set
			{
				_IsShowNameInHeader = value;
				OnPropertyChanged(() => IsShowNameInHeader);
			}
		}

		bool _IsShowPeriod;
		public bool IsShowPeriod
		{
			get { return _IsShowPeriod; }
			set
			{
				_IsShowPeriod = value;
				OnPropertyChanged(() => IsShowPeriod);
			}
		}

		bool _IsShowDate;
		public bool IsShowDate
		{
			get { return _IsShowDate; }
			set
			{
				_IsShowDate = value;
				OnPropertyChanged(() => IsShowDate);
			}
		}

		bool _IsShowUser;
		public bool IsShowUser
		{
			get { return _IsShowUser; }
			set
			{
				_IsShowUser = value;
				OnPropertyChanged(() => IsShowUser);
			}
		}

		bool _IsWithDeleted;
		public bool IsWithDeleted
		{
			get { return _IsWithDeleted; }
			set
			{
				_IsWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
				DepartmentsFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter.DepartmentUIDs, DeletationType);
				PositionsFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter.PositionUIDs, DeletationType);
				EmployeesFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter, DeletationType, CurrentPersonType);
			}
		}

		public LogicalDeletationType DeletationType
		{
			get{ return IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;}
		}
		
		bool _IsGuests;
		public bool IsGuests
		{
			get { return _IsGuests; }
			set
			{
				_IsGuests = value;
				OnPropertyChanged(() => IsGuests);
				OnPropertyChanged(() => EmployeeTabItemHeader);
				EmployeesFilterViewModel.Initialize(SelectedCardReportFilter.EmployeeFilter, DeletationType, CurrentPersonType);
			}
		}

		public string EmployeeTabItemHeader
		{
			get { return IsGuests ? "Посетители" : "Сотрудники"; }
		}

		public PersonType CurrentPersonType
		{
			get{ return IsGuests ? PersonType.Guest : PersonType.Employee;}
		}
		

		DateTime _EndDate;
		public DateTime EndDate
		{
			get { return _EndDate; }
			set
			{
				_EndDate = value;
				OnPropertyChanged(() => EndDate);
				OnPropertyChanged(() => EndDateString);
			}
		}

		public string EndDateString
		{
			get { return EndDate.ToString("dd/MM/yyyy"); }
		}

		public string SelectShowAdditionalString
		{
			get { return IsShowAdditional ? "Скрыть настройки отображения отчёта" : "Показать настройки отображения отчёта"; }
		}

		#region Commands
		public RelayCommand SelectShowAdditionalCommand { get; private set; }
		void OnSelectShowAdditional()
		{
			IsShowAdditional = !IsShowAdditional;
		}

		public RelayCommand SelectEndDateCommand { get; private set; }
		void OnSelectEndDate()
		{
			var selectDateViewModel = new DateSelectionViewModel(EndDate);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				EndDate = selectDateViewModel.DateTime;
				SelectedEndDateType = EndDateType.Arbitrary;
			}
		}

		public RelayCommand SaveFilterCommand { get; private set; }
		void OnSaveFilter()
		{
			var name = SelectedCardReportFilter.Name;
			if (name == "По умолчанию")
				name = CopyHelper.CopyName("Новый фильтр", CardReportFilters.Select(x => x.Name));
			var reportNameViewModel = new ReportNameViewModel(name);
			if (DialogService.ShowModalWindow(reportNameViewModel))
			{
				var newName = reportNameViewModel.Text;
				if (!newName.Equals(SelectedCardReportFilter.Name))
				{
					var cardReportFilter = new CardReportFilter();
					cardReportFilter.Name = newName;
					CardReportFilters.Add(cardReportFilter);
					_SelectedCardReportFilter = cardReportFilter;
					OnPropertyChanged(() => SelectedCardReportFilter);
				}
				CopyPropertiesBack();
			}
		}

		public RelayCommand RemoveFilterCommand { get; private set; }
		void OnRemoveFilter()
		{
			CardReportFilters.Remove(SelectedCardReportFilter);
			SelectedCardReportFilter = CardReportFilters.FirstOrDefault();
		}
		#endregion
	}

}
