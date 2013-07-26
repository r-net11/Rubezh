using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common.Ribbon;
using System.Collections.Generic;

namespace FiltersModule.ViewModels
{
	public class FiltersViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public FiltersViewModel()
		{
			Menu = new FiltersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
            RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>(
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Select(journalFilter => new FilterViewModel(journalFilter))
			);
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
				var filter = filterDetailsViewModel.GetModel();
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filter);
                var filterViewModel = new FilterViewModel(filter);
                Filters.Add(filterViewModel);
                SelectedFilter = filterViewModel;
				ServiceFactory.SaveService.FilterChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter);
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterDetailsViewModel.GetModel());
				SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();
				ServiceFactory.SaveService.FilterChanged = true;
			}
		}

		bool CanEditRemove()
		{
			return SelectedFilter != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
			Filters.Remove(SelectedFilter);
			SelectedFilter = Filters.FirstOrDefault();
			ServiceFactory.SaveService.FilterChanged = true;
		}

        void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BFilter.png") { Order = 2 }
			};
		}
	}
}