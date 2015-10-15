using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

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
		}

		public DevicesViewModel DevicesViewModel { get; private set; }
		public ConsumersViewModel ConsumersViewModel { get; private set; }
		public ReportsViewModel ReportsViewModel { get; private set; }
		public UsersViewModel UsersViewModel { get; private set; }
		public JournalEventsViewModel JournalEventsViewModel { get; private set; }
		public TariffsViewModel TariffsViewModel { get; private set; }
		public SettingsViewModel SettingsViewModel { get; private set; }
    }
}