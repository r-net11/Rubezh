using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using System.Windows.Input;
using Localization.Filter.Common;
using Localization.Filter.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using System.Collections.Generic;
using Infrastructure.Common.Ribbon;

namespace FiltersModule.ViewModels
{
	public class FiltersViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public FiltersViewModel()
		{
			Menu = new FiltersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
			ServiceFactory.Events.GetEvent<CreateFilterEvent>().Unsubscribe(OnAdd);
			ServiceFactory.Events.GetEvent<CreateFilterEvent>().Subscribe(OnAdd);
		}

		public void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in FiresecClient.FiresecManager.SystemConfiguration.JournalFilters)
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
				OnPropertyChanged(() => Filters);
			}
		}

		FilterViewModel _selectedFilter;
		public FilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel();
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterDetailsViewModel.Filter);
				ServiceFactory.SaveService.FilterChanged = true;
				var filterViewModel = new FilterViewModel(filterDetailsViewModel.Filter);
				Filters.Add(filterViewModel);
				SelectedFilter = filterViewModel;
			}
		}

		void OnAdd(object obj)
		{
			OnAdd();
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Logger.Info(String.Format("Удаление фильтра журнала событий GUID='{0}' Название='{1}'", SelectedFilter.Filter.UID, SelectedFilter.Filter.Name));
			var deletedFilterID = SelectedFilter.Filter.UID;
			var index = Filters.IndexOf(SelectedFilter);
			FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.Filter);
			Filters.Remove(SelectedFilter);
			index = Math.Min(index, Filters.Count - 1);
			if (index > -1)
				SelectedFilter = Filters[index];
			ServiceFactory.SaveService.FilterChanged = true;
			ServiceFactoryBase.Events.GetEvent<FilterDeletedEvent>().Publish(deletedFilterID);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.Filter);
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				SelectedFilter.Update(filterDetailsViewModel.Filter);
				ServiceFactory.SaveService.FilterChanged = true;
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
				SelectedFilter = Filters.FirstOrDefault(item => item.Filter.UID == filterUid);
			}
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
					new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel(CommonResources.Add, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(CommonResources.Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Delete, DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}