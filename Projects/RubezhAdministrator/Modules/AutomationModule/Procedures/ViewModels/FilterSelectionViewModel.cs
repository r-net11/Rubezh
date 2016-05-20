using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Automation;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace AutomationModule.ViewModels
{
	public class FilterSelectionViewModel : SaveCancelDialogViewModel
	{
		Procedure Procedure { get; set; }

		public FilterSelectionViewModel(Procedure procedure)
		{
			Title = "Выбор фильтра";
			Procedure = procedure;
			InitializeFilters();
			CreateFilterCommand = new RelayCommand(OnCreateFilter);
		}

		void InitializeFilters()
		{			
			if (Filters == null)
				Filters = new ObservableCollection<FilterViewModel>();
			else
				Filters.Clear();

			foreach (var filter in ClientManager.SystemConfiguration.JournalFilters.FindAll(x => !Procedure.FiltersUids.Contains(x.UID)))
			{
				var filterViewModel = new FilterViewModel(filter);
				Filters.Add(filterViewModel);
			}
			OnPropertyChanged(() => Filters);
			SelectedFilter = Filters.FirstOrDefault();
		}

		public RelayCommand CreateFilterCommand { get; private set; }
		void OnCreateFilter()
		{
			ServiceFactoryBase.Events.GetEvent<CreateFilterEvent>().Publish(new object());

			var all = ClientManager.SystemConfiguration.JournalFilters.FindAll(x => !Procedure.FiltersUids.Contains(x.UID));
			var newFilters = all.Where(f => !Filters.Any(x => x.Filter.UID == f.UID)).Select(p => new FilterViewModel(p));

			foreach (var filter in newFilters)
			{
				Filters.Add(filter);
			}

			SelectedFilter = Filters.Last();
		}

		public ObservableCollection<FilterViewModel> Filters { get; private set; }

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

		protected override bool CanSave()
		{
			return SelectedFilter != null;
		}
	}
}
