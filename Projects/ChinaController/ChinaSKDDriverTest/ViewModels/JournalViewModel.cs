using System;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using StrazhDeviceSDK;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		void Wrapper_NewJournalItem(SKDJournalItem journalItem)
		{
			ApplicationService.BeginInvoke(new Action(() =>
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}));
		}

		public ObservableCollection<JournalItemViewModel> JournalItems { get; private set; }

		JournalItemViewModel _selectedJournalItem;
		public JournalItemViewModel SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged(() => SelectedJournalItem);
			}
		}
	}
}