using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using StrazhDeviceSDK.API;
using ControllerSDK.Events;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessLogItemsViewModel : BaseViewModel
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

		public AccessLogItemsViewModel()
		{
			LogDeepDateTime = DateTime.Now;
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			GenerateJournalItemCommand = new RelayCommand(OnGenerateJournalItem, CanGenerateJournalItem);
			GetLogItemsUsingLogDeepCommand = new RelayCommand(OnGetLogItemsUsingLogDeep);
			AccessLogItems = new ObservableCollection<AccessLogItemViewModel>();
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			MessageBox.Show("Всего проходов: " + MainViewModel.Wrapper.GetAccessLogItemsCount());
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var accesses = MainViewModel.Wrapper.GetAllAccessLogItems();

			FillAccessLogItems(accesses);
		}

		public RelayCommand GenerateJournalItemCommand { get; private set; }
		private void OnGenerateJournalItem()
		{
			if (SelectedAccessLogItem == null)
				return;

			var journalItem = SelectedAccessLogItem.TransformToJournalItem();
			ServiceFactory.Instance.Events.GetEvent<JournalItemEvent>().Publish(journalItem);
		}
		private bool CanGenerateJournalItem()
		{
			return SelectedAccessLogItem != null;
		}

		public RelayCommand GetLogItemsUsingLogDeepCommand { get; private set; }
		private void OnGetLogItemsUsingLogDeep()
		{
			var accesses = MainViewModel.Wrapper.GetAccessLogItemsOlderThan(LogDeepDateTime);

			FillAccessLogItems(accesses);
		}

		private void FillAccessLogItems(IEnumerable<AccessLogItem> accesses)
		{
			AccessLogItems.Clear();
			foreach (var access in accesses)
			{
				AccessLogItems.Add(new AccessLogItemViewModel(access));
			}
		}

		public ObservableCollection<AccessLogItemViewModel> AccessLogItems { get; private set; }

		AccessLogItemViewModel _selectedAccessLogItem;
		public AccessLogItemViewModel SelectedAccessLogItem
		{
			get { return _selectedAccessLogItem; }
			set
			{
				_selectedAccessLogItem = value;
				OnPropertyChanged(() => SelectedAccessLogItem);
			}
		}
	}
}