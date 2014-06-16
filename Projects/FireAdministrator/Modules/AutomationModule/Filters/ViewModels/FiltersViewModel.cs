using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace AutomationModule.ViewModels
{
	public class FiltersViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public FiltersViewModel()
		{
			Menu = new FiltersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.Filters == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.Filters = new List<AutomationFilter>();
			foreach (var filter in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.Filters)
			{
				var filterViewModel = new FilterViewModel(filter);
				Filters.Add(filterViewModel);
			}
			SelectedFilter = Filters.FirstOrDefault();
		}

		ObservableCollection<FilterViewModel> _filters;
		public ObservableCollection<FilterViewModel> Filters
		{
			get { return _filters; }
			set
			{
				_filters = value;
				OnPropertyChanged("Filters");
			}
		}

		FilterViewModel _selectedFilter;
		public FilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged("SelectedFilter");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel();
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.Filters.Add(filterDetailsViewModel.Filter);
				ServiceFactory.SaveService.AutomationChanged = true;
				var filterViewModel = new FilterViewModel(filterDetailsViewModel.Filter);
				Filters.Add(filterViewModel);
				SelectedFilter = filterViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.Filters.Remove(SelectedFilter.Filter);
			Filters.Remove(SelectedFilter);
			SelectedFilter = Filters.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.Filter);
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				SelectedFilter.Update(filterDetailsViewModel.Filter);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedFilter != null;
		}

		public void Select(Guid filterUid)
		{
			if (filterUid != Guid.Empty)
			{
				SelectedFilter = Filters.FirstOrDefault(item => item.Filter.Uid == filterUid);
			}
		}
	}
}