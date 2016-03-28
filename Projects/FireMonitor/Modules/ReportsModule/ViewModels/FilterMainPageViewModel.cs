using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ReportsModule.ViewModels
{
	public class FilterMainPageViewModel : FilterContainerViewModel
	{
		private readonly Action<SKDReportFilter> _updateFilterAction;
		private readonly Action<SKDReportFilter> _loadFilterAction;
		private readonly Type _filterType;
		private bool _isLoaded;
		private readonly SKDReportFilter _filter;
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
			Filters = new ObservableCollection<SKDReportFilter>(FiresecManager.FiresecService.GetReportFiltersByType(FiresecManager.CurrentUser, ((SKDReportFilter)Activator.CreateInstance(_filterType)).ReportType).Result);
			Filters.Insert(0, (SKDReportFilter)Activator.CreateInstance(_filterType));
			SelectedFilter = Filters.FirstOrDefault(f => f.Name == filter.Name) ?? Filters[0];
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
			_isLoaded = false;
			var filter = (SKDReportFilter)Activator.CreateInstance(_filterType);
			_updateFilterAction(filter);
			filter.Name = FilterName.Trim();
			var existFilter = Filters.FirstOrDefault(x => x.GetType() == _filterType && x.Name == FilterName);
			if (existFilter != null)
			{
				Filters.Remove(existFilter);
			}

			var saveResult = FiresecManager.FiresecService.SaveReportFilter(filter, FiresecManager.CurrentUser);
			if (saveResult.Result)
			{
				Filters.Add(filter);
				SelectedFilter = filter;
			}
			_isLoaded = true;
		}
		private bool CanSaveFilter()
		{
			return !string.IsNullOrEmpty(FilterName.Trim()) && FilterName.Trim() != Filters[0].Name;
		}
		public RelayCommand RemoveFilterCommand { get; private set; }
		private void OnRemoveFilter()
		{
			var removeResult = FiresecManager.FiresecService.RemoveReportFilter(SelectedFilter, FiresecManager.CurrentUser);

			if (!removeResult.Result) return;

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
							DateTimeFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month -1, 1);
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
				ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Publish(value);
			}
		}

		private bool _hasBalanceVariation;

		public bool HasBalanceVariation
		{
			get { return _hasBalanceVariation; }
			set
			{
				if (_hasBalanceVariation == value) return;

				_hasBalanceVariation = value;
				OnPropertyChanged(() => HasBalanceVariation);
			}
		}

		private bool _allowOnlyAcceptedOvertime;

		public bool AllowOnlyAcceptedOvertime
		{
			get { return _allowOnlyAcceptedOvertime; }
			set
			{
				if (_allowOnlyAcceptedOvertime == value) return;

				_allowOnlyAcceptedOvertime = value;
				OnPropertyChanged(() => AllowOnlyAcceptedOvertime);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			PrintFilterName = filter.PrintFilterName;
			PrintFilterNameInHeader = filter.PrintFilterNameInHeader;
			PrintPeriod = filter.PrintPeriod;
			PrintDate = filter.PrintDate;
			PrintUser = filter.PrintUser;

			var workingTimeFilter = filter as WorkingTimeReportFilter;
			if (workingTimeFilter != null)
			{
				HasBalanceVariation = true;
				AllowOnlyAcceptedOvertime = workingTimeFilter.AllowOnlyAcceptedOvertime;
			}

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
				UseArchive = ((IReportFilterArchive)filter).UseArchive;
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

			if (filter is WorkingTimeReportFilter)
				((WorkingTimeReportFilter) filter).AllowOnlyAcceptedOvertime = AllowOnlyAcceptedOvertime;
		}
	}
}
