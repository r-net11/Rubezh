using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel
	{
		XDevice Device;
		const int MaxCount = 100;

		public JournalViewModel()
		{
			ServiceFactory.Events.GetEvent<NewJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewJournalEvent>().Subscribe(OnNewJournal);
			JournalItems = new ObservableCollection<JournalItem>();
		}

		public void Initialize()
		{
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);

			var lastId = JournalWatcher.GetLastId();
			for (int i = Math.Max(0, lastId - MaxCount); i <= lastId; i++)
			{
				OnNewJournal(i);
			}

			SelectedJournal = JournalItems.FirstOrDefault();
		}

		void OnNewJournal(int index)
		{
			var journalItem = ReadJournal(index);
			if (journalItem == null)
				return;

			if (JournalItems.Count > 0)
				JournalItems.Insert(0, journalItem);
			else
				JournalItems.Add(journalItem);

			if (JournalItems.Count > MaxCount)
				JournalItems.RemoveAt(MaxCount);
		}

		ObservableCollection<JournalItem> _journalItems;
		public ObservableCollection<JournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		JournalItem _selectedJournal;
		public JournalItem SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged("SelectedJournal");
			}
		}

		JournalItem ReadJournal(int index)
		{
			var data = new List<byte>();
			data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(Device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				return null;
			}
			var journalItem = new JournalItem(sendResult.Bytes);
			return journalItem;
		}
	}
}