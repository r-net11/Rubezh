using System.Collections.ObjectModel;
using StrazhDeviceSDK;
using StrazhDeviceSDK.API;
using ControllerSDK.Events;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace ControllerSDK.ViewModels
{
	public class NativeJournalViewModel : BaseViewModel
	{
		public NativeJournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Wrapper.NewJournalItem += AddOnlineJournalItem;
			ServiceFactory.Instance.Events.GetEvent<JournalItemEvent>().Subscribe(AddOfflineJournalItem);
		}

		private void AddJournalItem(SKDJournalItem journalItem, JournalItemType journalItemType)
		{
			var journalItemViewModel = new JournalItemViewModel(journalItem, journalItemType);
			ApplicationService.BeginInvoke(() => JournalItems.Add(journalItemViewModel));
		}

		private void AddOnlineJournalItem(SKDJournalItem journalItem)
		{
			AddJournalItem(journalItem, JournalItemType.Online);
		}

		private void AddOfflineJournalItem(SKDJournalItem journalItem)
		{
			AddJournalItem(journalItem, JournalItemType.Offline);
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