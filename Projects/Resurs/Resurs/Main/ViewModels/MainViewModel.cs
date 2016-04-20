using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			DevicesViewModel = new DevicesViewModel();
			ConsumersViewModel = new ConsumersViewModel();
			ReportsViewModel = new ReportsViewModel();
			UsersViewModel = new UsersViewModel();
			JournalEventsViewModel = new JournalEventsViewModel();
			TariffsViewModel = new TariffsViewModel();
			SettingsViewModel = new SettingsViewModel();
			ReceiptEditorViewModel = new ReceiptEditorViewModel();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }
		public ConsumersViewModel ConsumersViewModel { get; private set; }
		public ReportsViewModel ReportsViewModel { get; private set; }
		public UsersViewModel UsersViewModel { get; private set; }
		public JournalEventsViewModel JournalEventsViewModel { get; private set; }
		public TariffsViewModel TariffsViewModel { get; private set; }
		public SettingsViewModel SettingsViewModel { get; private set; }
		public ReceiptEditorViewModel ReceiptEditorViewModel { get; private set; }

		int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged(() => SelectedTabIndex);
			}
		}

		public void UpdateTabsIsVisible()
		{
			DevicesViewModel.OnPropertyChanged(() => DevicesViewModel.IsVisible);
			ConsumersViewModel.OnPropertyChanged(() => ConsumersViewModel.IsVisible);
			UsersViewModel.OnPropertyChanged(() => UsersViewModel.IsVisible);
			JournalEventsViewModel.OnPropertyChanged(() => JournalEventsViewModel.IsVisible);
			TariffsViewModel.OnPropertyChanged(() => TariffsViewModel.IsVisible);
		}
    }
}