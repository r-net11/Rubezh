using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using StrazhAPI.Automation;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ConditionsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public ConditionsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			Initialize();
			
			ServiceFactoryBase.Events.GetEvent<FilterDeletedEvent>().Subscribe(FilterDeletedEventHandler);
		}

		private void FilterDeletedEventHandler(Guid filterID)
		{
			var filter = Filters.FirstOrDefault(x => x.Filter.UID == filterID);
			if (filter == null)
				return;

			DeleteFilter(filter);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		private void DeleteFilter(FilterViewModel filter)
		{
			Logger.Info(String.Format("Процедура автоматизации GUID='{0}' Название='{1}'. Удаление ссылки на фильтр журнала событий GUID='{2}' Название='{3}'",
				Procedure.Uid,
				Procedure.Name,
				filter.Filter.UID,
				filter.Filter.Name));
			Procedure.FiltersUids.Remove(filter.Filter.UID);
			Filters.Remove(filter);
		}

		private void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				if (Procedure.FiltersUids.Contains(filter.UID))
				{
					var filterViewModel = new FilterViewModel(filter);
					Filters.Add(filterViewModel);
				}
			}
		}

		public ObservableCollection<FilterViewModel> Filters { get; private set; }

		private FilterViewModel _selectedFilter;
		public FilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		public ICommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var procedureFilterDetailsViewModel = new FilterSelectionViewModel(Procedure);
			if (DialogService.ShowModalWindow(procedureFilterDetailsViewModel))
			{
				var filterViewModel = procedureFilterDetailsViewModel.SelectedFilter;
				Filters.Add(filterViewModel);
				SelectedFilter = filterViewModel;
				Procedure.FiltersUids.Add(filterViewModel.Filter.UID);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ICommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			DeleteFilter(SelectedFilter);
			SelectedFilter = Filters.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		private bool CanDelete()
		{
			return SelectedFilter != null;
		}
	}
}