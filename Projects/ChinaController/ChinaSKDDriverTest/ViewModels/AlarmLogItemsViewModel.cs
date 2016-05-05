using System;
using System.Collections.Generic;
using ControllerSDK.Events;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using StrazhDeviceSDK.API;
using System.Collections.ObjectModel;

namespace ControllerSDK.ViewModels
{
	public class AlarmLogItemsViewModel : BaseViewModel
	{
		private DateTime _logDeepDateTime;
		public DateTime LogDeepDateTime
		{
			get { return _logDeepDateTime; }
			set
			{
				if (_logDeepDateTime == value)
					return;
				_logDeepDateTime = value;
				OnPropertyChanged(() => LogDeepDateTime);
			}
		}

		public AlarmLogItemsViewModel()
		{
			LogDeepDateTime = DateTime.Now;
			GetLogsCountCommand = new RelayCommand(OnGetLogsCount);
			GetAllLogsCommand = new RelayCommand(OnGetAllLogs);
			GenerateJournalItemCommand = new RelayCommand(OnGenerateJournalItem, CanGenerateJournalItem);
			GetLogItemsUsingLogDeepCommand = new RelayCommand(OnGetLogItemsUsingLogDeep);
			AlarmLogItems = new ObservableCollection<AlarmLogItemViewModel>();
		}

		public RelayCommand GetLogsCountCommand { get; private set; }
		void OnGetLogsCount()
		{
			MessageBox.Show("Всего тревог: " + MainViewModel.Wrapper.GetAlarmLogItemsCount());
		}

		public RelayCommand GetLogItemsUsingLogDeepCommand { get; private set; }
		private void OnGetLogItemsUsingLogDeep()
		{
			var alarms = MainViewModel.Wrapper.GetAlarmLogItemsOlderThan(LogDeepDateTime);

			FillAlarmLogItems(alarms);
		}

		public ObservableCollection<AlarmLogItemViewModel> AlarmLogItems { get; private set; }

		private AlarmLogItemViewModel _selectedAlarmLogItem;
		public AlarmLogItemViewModel SelectedAlarmLogItem {
			get { return _selectedAlarmLogItem; }
			set
			{
				if (_selectedAlarmLogItem == value)
					return;
				_selectedAlarmLogItem = value;
				OnPropertyChanged(() => SelectedAlarmLogItem);
			}
		}

		public RelayCommand GetAllLogsCommand { get; private set; }
		void OnGetAllLogs()
		{
			var alarmLogItems = MainViewModel.Wrapper.GetAllAlarmLogItems();

			FillAlarmLogItems(alarmLogItems);
		}

		private void FillAlarmLogItems(IEnumerable<AlarmLogItem> alarmLogItems)
		{
			AlarmLogItems.Clear();
			foreach (var alarmLogItem in alarmLogItems)
			{
				AlarmLogItems.Add(new AlarmLogItemViewModel(alarmLogItem));
			}
		}

		public RelayCommand GenerateJournalItemCommand { get; private set; }
		private void OnGenerateJournalItem()
		{
			if (SelectedAlarmLogItem == null)
				return;

			var journalItem = SelectedAlarmLogItem.TransformToJournalItem();
			ServiceFactory.Instance.Events.GetEvent<JournalItemEvent>().Publish(journalItem);
		}
		private bool CanGenerateJournalItem()
		{
			return SelectedAlarmLogItem != null;
		}
	}
}