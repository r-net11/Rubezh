﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriverNativeApi;
using Infrastructure.Common.Windows;
using Microsoft.Practices.Prism;

namespace ControllerSDK.ViewModels
{
	public class OfflineLogItemsViewModel : BaseViewModel
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

		public OfflineLogItemsViewModel()
		{
			LogDeepDateTime = DateTime.Now;
			OfflineLogItems = new ObservableCollection<OfflineLogItemViewModel>();
			GetLogItemsUsingLogDeepCommand = new RelayCommand(OnGetLogItemsUsingLogDeep);
		}

		public RelayCommand GetLogItemsUsingLogDeepCommand { get; private set; }
		private void OnGetLogItemsUsingLogDeep()
		{
			var offlineLogItems = MainViewModel.Wrapper.GetOfflineLogItems(LogDeepDateTime);

			OfflineLogItems.Clear();
			foreach (var item in offlineLogItems)
			{
				OfflineLogItems.Add(new OfflineLogItemViewModel(item));
			}
		}

		public ObservableCollection<OfflineLogItemViewModel> OfflineLogItems { get; private set; }
	}
}