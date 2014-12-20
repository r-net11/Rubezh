using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using Common;
using System.Collections.ObjectModel;

namespace ReportsModule.ViewModels
{
	public class SKDReportFilterViewModel : SaveCancelDialogViewModel
	{
		public SKDReportFilter Filter { get; private set; }
		private FilterModel _model;

		public SKDReportFilterViewModel(SKDReportFilter filter, FilterModel model)
		{
			Title = "Настройки отчета";
			_model = model;
			Filter = filter;
			Pages = new ObservableCollection<FilterContainerViewModel>();
			Pages.Add(new FilterMainViewModel());
			model.Pages.ForEach(page => Pages.Add(page));
			if (model.AllowSort)
				Pages.Add(new FilterSortViewModel(model.Columns));
			Pages.ForEach(page => page.LoadFilter(filter));
		}

		private ObservableCollection<FilterContainerViewModel> _pages;
		public ObservableCollection<FilterContainerViewModel> Pages
		{
			get { return _pages; }
			set
			{
				_pages = value;
				OnPropertyChanged(() => Pages);
			}
		}


		protected override bool Save()
		{
			// UpdateFilter -> sort + common
			if (_model.CommandsViewModel != null)
				_model.CommandsViewModel.UpdateFilter(Filter);
			if (_model.Pages != null)
				_model.Pages.ForEach(page => page.UpdateFilter(Filter));
			return base.Save();
		}
	}
}
