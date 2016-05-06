using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
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
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in FiresecManager.SystemConfiguration.JournalFilters.FindAll(x => !Procedure.FiltersUids.Contains(x.UID)))
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
			InitializeFilters();
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

		protected override bool Save()
		{
			return SelectedFilter != null;
		}
	}
}
