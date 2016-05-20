using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient.SKDHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ReportsModule.ViewModels
{
	public class FilterMainPageViewModel : FilterContainerViewModel
	{
		private Action<SKDReportFilter> _updateFilterAction;
		private Action<SKDReportFilter> _loadFilterAction;
		private Type _filterType;
		private bool _isLoaded;
		private SKDReportFilter _filter;
		public FilterMainPageViewModel(FilterModel model, SKDReportFilter filter, Action<SKDReportFilter> loadFilterAction, Action<SKDReportFilter> updateFilterAction)
		{
			_isLoaded = false;
			Title = "Настройки";
			_filter = filter;
			_loadFilterAction = loadFilterAction;
			_updateFilterAction = updateFilterAction;
			MainViewModel = model.MainViewModel;
			IsAdditionalExpanded = false;
			ExpandAdditionalCommand = new RelayCommand(() => IsAdditionalExpanded = !IsAdditionalExpanded);
			SaveFilterCommand = new RelayCommand(OnSaveFilter, CanSaveFilter);
			RemoveFilterCommand = new RelayCommand(OnRemoveFilter, CanRemoveFilter);
			_filterType = filter.GetType();
			Filters = new ObservableCollection<SKDReportFilter>(ClientSettings.ReportFilters.Filters.Where(f => f.GetType() == _filterType));
			Filters.Insert(0, (SKDReportFilter)Activator.CreateInstance(_filterType));
			SelectedFilter = Filters.FirstOrDefault(f => f.Name == filter.Name);
			if (SelectedFilter == null)
				SelectedFilter = Filters[0];
			_isLoaded = true;
		}

		public FilterContainerViewModel MainViewModel { get; private set; }
		public RelayCommand ExpandAdditionalCommand { get; private set; }

		public ObservableCollection<SKDReportFilter> Filters { get; private set; }
		private SKDReportFilter _selectedFilter;
		public SKDReportFilter SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
				if (SelectedFilter != null)
				{
					FilterName = SelectedFilter.Name;
					_filter.Name = FilterName;
					if (_isLoaded)
						_loadFilterAction(SelectedFilter);
				}
			}
		}
		private string _filterName;
		public string FilterName
		{
			get { return _filterName; }
			set
			{
				_filterName = value;
				OnPropertyChanged(() => FilterName);
			}
		}

		public RelayCommand SaveFilterCommand { get; private set; }
		private void OnSaveFilter()
		{
			if (FilterName.Length > 50)
			{
				MessageBoxService.Show("Название фильтра не может быть длиннее 50 символов");
				return;
			}
			_isLoaded = false;
			var filter = (SKDReportFilter)Activator.CreateInstance(_filterType);
			_updateFilterAction(filter);
			filter.Name = FilterName.Trim();
			var existFilter = ClientSettings.ReportFilters.Filters.FirstOrDefault(f => f.GetType() == _filterType && f.Name == FilterName);
			if (existFilter != null)
			{
				ClientSettings.ReportFilters.Filters.Remove(existFilter);
				Filters.Remove(existFilter);
			}
			ClientSettings.ReportFilters.Filters.Add(filter);
			Filters.Add(filter);
			SelectedFilter = filter;
			_isLoaded = true;
		}
		private bool CanSaveFilter()
		{
			return !string.IsNullOrEmpty(FilterName.Trim()) && FilterName.Trim() != Filters[0].Name;
		}
		public RelayCommand RemoveFilterCommand { get; private set; }
		private void OnRemoveFilter()
		{
			if (ClientSettings.ReportFilters.Filters.Contains(SelectedFilter))
				ClientSettings.ReportFilters.Filters.Remove(SelectedFilter);
			Filters.Remove(SelectedFilter);
			SelectedFilter = Filters[0];
		}
		private bool CanRemoveFilter()
		{
			return Filters.IndexOf(SelectedFilter) > 0;
		}

		private bool _isAdditionalExpanded;
		public bool IsAdditionalExpanded
		{
			get { return _isAdditionalExpanded; }
			set
			{
				_isAdditionalExpanded = value;
				OnPropertyChanged(() => IsAdditionalExpanded);
			}
		}

		private bool _printFilterName;
		public bool PrintFilterName
		{
			get { return _printFilterName; }
			set
			{
				_printFilterName = value;
				OnPropertyChanged(() => PrintFilterName);
			}
		}
		private bool _printFilterNameInHeader;
		public bool PrintFilterNameInHeader
		{
			get { return _printFilterNameInHeader; }
			set
			{
				_printFilterNameInHeader = value;
				OnPropertyChanged(() => PrintFilterNameInHeader);
			}
		}
		private bool _printPeriod;
		public bool PrintPeriod
		{
			get { return _printPeriod; }
			set
			{
				_printPeriod = value;
				OnPropertyChanged(() => PrintPeriod);
			}
		}
		private bool _printDate;
		public bool PrintDate
		{
			get { return _printDate; }
			set
			{
				_printDate = value;
				OnPropertyChanged(() => PrintDate);
			}
		}
		private bool _printUser;
		public bool PrintUser
		{
			get { return _printUser; }
			set
			{
				_printUser = value;
				OnPropertyChanged(() => PrintUser);
			}
		}

		private bool _hasPeriod;
		public bool HasPeriod
		{
			get { return _hasPeriod; }
			set
			{
				_hasPeriod = value;
				OnPropertyChanged(() => HasPeriod);
			}
		}
		private ReportPeriodType _selectedReportPeriod;
		public ReportPeriodType SelectedReportPeriod
		{
			get { return _selectedReportPeriod; }
			set
			{
				_selectedReportPeriod = value;
				OnPropertyChanged(() => SelectedReportPeriod);
				if (_isLoaded)
					switch (SelectedReportPeriod)
					{
						case ReportPeriodType.Day:
							_isLoaded = false;
							DateTimeFrom = DateTime.Today.AddDays(-1);
							DateTimeTo = DateTime.Today.AddSeconds(-1);
							_isLoaded = true;
							break;
						case ReportPeriodType.Week:
							_isLoaded = false;
							DateTimeFrom = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek).AddDays(-7);
							DateTimeTo = DateTimeFrom.AddDays(6);
							_isLoaded = true;
							break;
						case ReportPeriodType.Month:
							_isLoaded = false;
							DateTimeFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, 1);
							DateTimeTo = DateTimeFrom.AddDays(DateTime.DaysInMonth(DateTimeFrom.Year, DateTimeFrom.Month) - 1);
							_isLoaded = true;
							break;
					}
			}
		}
		private DateTime _dateTimeFrom;
		public DateTime DateTimeFrom
		{
			get { return _dateTimeFrom; }
			set
			{
				_dateTimeFrom = value;
				OnPropertyChanged(() => DateTimeFrom);
				if (_isLoaded)
					SelectedReportPeriod = ReportPeriodType.Arbitrary;
			}
		}
		private DateTime _dateTimeTo;
		public DateTime DateTimeTo
		{
			get { return _dateTimeTo; }
			set
			{
				_dateTimeTo = value;
				OnPropertyChanged(() => DateTimeTo);
				if (_isLoaded)
					SelectedReportPeriod = ReportPeriodType.Arbitrary;

				if (DateTimeFrom > DateTimeTo)
					DateTimeFrom = DateTimeTo;
			}
		}

		DateTime _minDate;
		public DateTime MinDate
		{
			get { return _minDate; }
			set
			{
				_minDate = value;
				OnPropertyChanged(() => MinDate);
			}
		}

		//public DateTime MaxDate { get { return DateTime.Now; } }
		public DateTime MaxDate { get { return DateTime.Today.AddDays(1).AddSeconds(-1); } }

		private bool _hasArchive;
		public bool HasArchive
		{
			get { return _hasArchive; }
			set
			{
				_hasArchive = value;
				OnPropertyChanged(() => HasArchive);
			}
		}
		private bool _useArchive;
		public bool UseArchive
		{
			get { return _useArchive; }
			set
			{
				_useArchive = value;
				OnPropertyChanged(() => UseArchive);
				ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Publish(value);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			PrintFilterName = filter.PrintFilterName;
			PrintFilterNameInHeader = filter.PrintFilterNameInHeader;
			PrintPeriod = filter.PrintPeriod;
			PrintDate = filter.PrintDate;
			PrintUser = filter.PrintUser;
			var periodFilter = filter as IReportFilterPeriod;
			HasPeriod = periodFilter != null;
			if (periodFilter != null)
			{
				var periodSet = periodFilter.DateTimeFrom != DateTime.MinValue && periodFilter.DateTimeTo != DateTime.MinValue;
				if (periodSet)
				{
					_isLoaded = false;
					SelectedReportPeriod = periodFilter.PeriodType;
					DateTimeFrom = periodFilter.DateTimeFrom;
					DateTimeTo = periodFilter.DateTimeTo;
					_isLoaded = true;
				}
				else
					SelectedReportPeriod = periodFilter.PeriodType;
				var minDate = filter is EventsReportFilter ? PassJournalHelper.GetMinJournalDate() : PassJournalHelper.GetMinPassJournalDate();
				MinDate = minDate != null ? minDate.Value : new DateTime();
			}
			HasArchive = filter is IReportFilterArchive;
			if (HasArchive)
				_useArchive = ((IReportFilterArchive)filter).UseArchive;
		}

		public override void UpdateFilter(SKDReportFilter filter)
		{
			filter.PrintFilterName = PrintFilterName;
			filter.PrintFilterNameInHeader = PrintFilterNameInHeader;
			filter.PrintPeriod = PrintPeriod;
			filter.PrintDate = PrintDate;
			filter.PrintUser = PrintUser;
			var periodFilter = filter as IReportFilterPeriod;
			if (periodFilter != null)
			{
				periodFilter.PeriodType = SelectedReportPeriod;
				periodFilter.DateTimeFrom = DateTimeFrom;
				periodFilter.DateTimeTo = DateTimeTo.Date.AddDays(1).AddSeconds(-1);
			}
			if (filter is IReportFilterArchive)
				((IReportFilterArchive)filter).UseArchive = UseArchive;
		}
	}
}
