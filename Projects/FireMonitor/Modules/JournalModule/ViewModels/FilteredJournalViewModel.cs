using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class FilteredJournalViewModel : BaseViewModel
	{
		public JournalFilter JournalFilter { get; private set; }

		public FilteredJournalViewModel(JournalFilter journalFilter)
		{
			ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Unsubscribe(OnNewJournalRecord);
			ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Subscribe(OnNewJournalRecord);

			if (journalFilter != null)
			{
				JournalFilter = journalFilter;
				Initialize();
			}
		}

		void Initialize()
		{
			var operationResult = FiresecManager.FiresecService.GetFilteredJournal(JournalFilter);
			JournalRecords = new ObservableCollection<JournalRecordViewModel>();
			if (operationResult.HasError == false)
			{
				foreach (var journalRecord in operationResult.Result)
				{
					var journalRecordViewModel = new JournalRecordViewModel(journalRecord);
					JournalRecords.Add(journalRecordViewModel);
				}
			}
		}

		public ObservableCollection<JournalRecordViewModel> JournalRecords { get; private set; }

		JournalRecordViewModel _selectedRecord;
		public JournalRecordViewModel SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				_selectedRecord = value;
				OnPropertyChanged("SelectedRecord");
			}
		}

		void OnNewJournalRecord(JournalRecord journalRecord)
		{
			if (FilterRecord(journalRecord) == false)
				return;

			if (JournalRecords.Count > 0)
				JournalRecords.Insert(0, new JournalRecordViewModel(journalRecord));
			else
				JournalRecords.Add(new JournalRecordViewModel(journalRecord));

			if (JournalRecords.Count > JournalFilter.LastRecordsCount)
				JournalRecords.RemoveAt(JournalFilter.LastRecordsCount);
		}

		bool FilterRecord(JournalRecord journalRecord)
		{
			if (JournalFilter.Categories.IsNotNullOrEmpty())
			{
				Device device = null;
				if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
				{
					device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
				}
				else
				{
					device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.PanelDatabaseId);
				}

				if (device != null)
				{
					if (JournalFilter.Categories.Any(daviceCategory => daviceCategory == device.Driver.Category) == false)
						return false;
				}
			}

			if (JournalFilter.StateTypes.IsNotNullOrEmpty())
			{
				if (JournalFilter.StateTypes.Any(x => x == journalRecord.StateType) == false)
					return false;
			}

			return true;
		}
	}
}