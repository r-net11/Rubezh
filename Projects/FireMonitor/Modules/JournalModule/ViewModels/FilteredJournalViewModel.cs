using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Firesec;
using FiresecAPI;
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
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Unsubscribe(OnNewJournalRecords);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Subscribe(OnNewJournalRecords);
			//ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Unsubscribe(OnNewFS2JournalItemsEvent);
			//ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Subscribe(OnNewFS2JournalItemsEvent);

			if (journalFilter != null)
			{
				JournalFilter = journalFilter;
				Initialize();
			}
		}

		void Initialize()
		{
			JournalRecords = new ObservableCollection<JournalRecordViewModel>();
			//if (FiresecManager.IsFS2Enabled)
			//{
			//	var operationResult = FiresecManager.FS2ClientContract.GetFilteredJournal(JournalFilter);
			//	if (operationResult.Result != null)
			//	{
			//		foreach (var journalItem in operationResult.Result)
			//		{
			//			var journalRecordViewModel = new JournalRecordViewModel(journalItem);
			//			JournalRecords.Add(journalRecordViewModel);
			//		}
			//	}
			//}
			//else
			{
				var operationResult = FiresecManager.FiresecService.GetFilteredJournal(JournalFilter);
				if (operationResult.Result != null)
				{
					foreach (var journalRecord in operationResult.Result)
					{
						JournalConverter.SetDeviceCatogoryAndDevieUID(journalRecord);
						var journalRecordViewModel = new JournalRecordViewModel(journalRecord);
						JournalRecords.Add(journalRecordViewModel);
					}
				}
			}
			SelectedRecord = JournalRecords.FirstOrDefault();
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

		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			foreach (var journalRecord in journalRecords)
			{
				if (FilterJournalRecord(journalRecord) == false)
					return;

				if (JournalRecords.Count > 0)
					JournalRecords.Insert(0, new JournalRecordViewModel(journalRecord));
				else
					JournalRecords.Add(new JournalRecordViewModel(journalRecord));

				if (JournalRecords.Count > JournalFilter.LastRecordsCount)
					JournalRecords.RemoveAt(JournalFilter.LastRecordsCount);
			}
		}

		//void OnNewFS2JournalItemsEvent(List<FS2JournalItem> journalItems)
		//{
		//	Dispatcher.BeginInvoke(new Action(() =>
		//		{
		//			foreach (var journalItem in journalItems)
		//			{
		//				if (FilterFS2JournalItem(journalItem) == false)
		//					return;

		//				if (JournalRecords.Count > 0)
		//					JournalRecords.Insert(0, new JournalRecordViewModel(journalItem));
		//				else
		//					JournalRecords.Add(new JournalRecordViewModel(journalItem));

		//				if (JournalRecords.Count > JournalFilter.LastRecordsCount)
		//					JournalRecords.RemoveAt(JournalFilter.LastRecordsCount);
		//			}
		//		}));
		//}

		bool FilterJournalRecord(JournalRecord journalRecord)
		{
			if (JournalFilter.Categories.IsNotNullOrEmpty())
			{
				Device device = null;
				if (journalRecord.DeviceDatabaseUID != Guid.Empty)
				{
					device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalRecord.DeviceDatabaseUID);
				}
				else
				{
					device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalRecord.PanelDatabaseUID);
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

		//bool FilterFS2JournalItem(FS2JournalItem journalItem)
		//{
		//	if (JournalFilter.Categories.IsNotNullOrEmpty())
		//	{
		//		Device device = null;
		//		if (journalItem.DeviceUID != Guid.Empty)
		//		{
		//			device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalItem.DeviceUID);
		//		}
		//		else
		//		{
		//			device = FiresecManager.Devices.FirstOrDefault(x => x.UID == journalItem.PanelUID);
		//		}

		//		if (device != null)
		//		{
		//			if (JournalFilter.Categories.Any(daviceCategory => daviceCategory == device.Driver.Category) == false)
		//				return false;
		//		}
		//	}

		//	if (JournalFilter.StateTypes.IsNotNullOrEmpty())
		//	{
		//		if (JournalFilter.StateTypes.Any(x => x == journalItem.StateType) == false)
		//			return false;
		//	}

		//	return true;
		//}
	}
}