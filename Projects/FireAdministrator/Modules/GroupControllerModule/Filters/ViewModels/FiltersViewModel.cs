using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class FiltersViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public FiltersViewModel()
		{
			Menu = new FiltersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            RegisterShortcuts();
		}

		public void Initialize()
		{
			JournalFilters = XManager.DeviceConfiguration.JournalFilters == null ? new ObservableCollection<FilterViewModel>() : new ObservableCollection<FilterViewModel>(
				from journalFilter in XManager.DeviceConfiguration.JournalFilters
				orderby journalFilter.Name
				select new FilterViewModel(journalFilter));
			SelectedJournalFilter = JournalFilters.FirstOrDefault();
		}

		ObservableCollection<FilterViewModel> _journalFilters;
		public ObservableCollection<FilterViewModel> JournalFilters
		{
			get { return _journalFilters; }
			set
			{
				_journalFilters = value;
				OnPropertyChanged("JournalFilters");
			}
		}

		FilterViewModel _selectedJournalFilter;
		public FilterViewModel SelectedJournalFilter
		{
			get { return _selectedJournalFilter; }
			set
			{
				_selectedJournalFilter = value;
				OnPropertyChanged("SelectedJournalFilter");
			}
		}

		bool CanEditDelete()
		{
			return SelectedJournalFilter != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel();
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				XManager.DeviceConfiguration.JournalFilters.Add(filterDetailsViewModel.JournalFilter);
                var filterViewModel = new FilterViewModel(filterDetailsViewModel.JournalFilter);
                JournalFilters.Add(filterViewModel);
                SelectedJournalFilter = filterViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить фильтр " + SelectedJournalFilter.JournalFilter.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				XManager.DeviceConfiguration.JournalFilters.Remove(SelectedJournalFilter.JournalFilter);
				JournalFilters.Remove(SelectedJournalFilter);
				SelectedJournalFilter = JournalFilters.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel(SelectedJournalFilter.JournalFilter);
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				SelectedJournalFilter.JournalFilter = filterDetailsViewModel.JournalFilter;
				SelectedJournalFilter.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedJournalFilter = SelectedJournalFilter;
		}

		public override void OnHide()
		{
			base.OnHide();
		}

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }
	}
}