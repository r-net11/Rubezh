using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class FilterSelectionViewModel : SaveCancelDialogViewModel
	{
		public FilterSelectionViewModel(Procedure procedure)
		{
			Title = "Выбор фильтра";
			Initialize(procedure);
		}

		void Initialize(Procedure procedure)
		{
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in FiresecManager.SystemConfiguration.JournalFilters.FindAll(x => !procedure.FiltersUids.Contains(x.UID)))
			{
				var filterViewModel = new FilterViewModel(filter);
				Filters.Add(filterViewModel);
			}
			SelectedFilter = Filters.FirstOrDefault();
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
			return true;
		}
	}
}
