﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using ChinaSKDDriverNativeApi;
using ChinaSKDDriverAPI;
using System.Collections.ObjectModel;

namespace ControllerSDK.ViewModels
{
	public class AlarmLogItemsViewModel : BaseViewModel
	{
		public AlarmLogItemsViewModel()
		{
			GetLogsCountCommand = new RelayCommand(OnGetLogsCount);
			GetAllLogsCommand = new RelayCommand(OnGetAllLogs);
		}

		public RelayCommand GetLogsCountCommand { get; private set; }
		void OnGetLogsCount()
		{
			MessageBox.Show("Всего тревог: " + MainViewModel.Wrapper.GetAlarmLogItemsCount());
		}

		ObservableCollection<AlarmLogItemViewModel> _alarmLogItems;
		public ObservableCollection<AlarmLogItemViewModel> AlarmLogItems
		{
			get { return _alarmLogItems; }
			set
			{
				_alarmLogItems = value;
				OnPropertyChanged(() => AlarmLogItems);
			}
		}

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
			AlarmLogItems = new ObservableCollection<AlarmLogItemViewModel>();
			var alarmLogItems = MainViewModel.Wrapper.GetAllAlarmLogItems();
			foreach (var alarmLogItem in alarmLogItems)
			{
				AlarmLogItems.Add(new AlarmLogItemViewModel(alarmLogItem));
			}
		}
	}
}