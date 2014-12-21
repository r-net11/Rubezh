using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;

namespace ReportsModule.ViewModels
{
	public class FilterMainViewModel : FilterContainerViewModel
	{
		private Action<SKDReportFilter> _updateFilterAction;
		private Action<SKDReportFilter> _loadFilterAction;
		private Type _filterType;
		private bool _isLoaded;
		public FilterMainViewModel(FilterModel model, SKDReportFilter filter, Action<SKDReportFilter> loadFilterAction, Action<SKDReportFilter> updateFilterAction)
		{
			_isLoaded = false;
			Title = "Настройки";
			_loadFilterAction = loadFilterAction;
			_updateFilterAction = updateFilterAction;
			MainViewModel = model.MainViewModel;
			HasPeriod = model.HasPeriod;
			IsAdditionalExpanded = false;
			ExpandAdditionalCommand = new RelayCommand(() => IsAdditionalExpanded = !IsAdditionalExpanded);
			SaveFilterCommand = new RelayCommand(OnSaveFilter);
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
		public bool HasPeriod { get; private set; }
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
			filter.Name = FilterName;
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
		public RelayCommand RemoveFilterCommand { get; private set; }
		private void OnRemoveFilter()
		{
			if (ClientSettings.ReportFilters.Filters.Contains(SelectedFilter))
				ClientSettings.ReportFilters.Filters.Remove(SelectedFilter);
			Filters.Remove(SelectedFilter);
			//SelectedFilter = Filters[0];
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

		private bool _printName;
		public bool PrintName
		{
			get { return _printName; }
			set
			{
				_printName = value;
				OnPropertyChanged(() => PrintName);
			}
		}
		private bool _printNameInHeader;
		public bool PrintNameInHeader
		{
			get { return _printNameInHeader; }
			set
			{
				_printNameInHeader = value;
				OnPropertyChanged(() => PrintNameInHeader);
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

		public override void LoadFilter(SKDReportFilter filter)
		{
			PrintName = filter.PrintName;
			PrintNameInHeader = filter.PrintNameInHeader;
			PrintPeriod = filter.PrintPeriod;
			PrintDate = filter.PrintDate;
			PrintUser = filter.PrintUser;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			filter.PrintName = PrintName;
			filter.PrintNameInHeader = PrintNameInHeader;
			filter.PrintPeriod = PrintPeriod;
			filter.PrintDate = PrintDate;
			filter.PrintUser = PrintUser;
		}
	}
}
