using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		XDevice Device;

		public JournalViewModel(XDevice device)
		{
			Title = "Журнал событий ГК";
			ReadCommand = new RelayCommand(OnRead);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Device = device;
			SetTotalCount();
			StartIndex = Math.Max(0, TotalCount - 100);
			EndIndex = TotalCount;
		}

		void SetTotalCount()
		{
			var sendResult = SendManager.Send(Device, 0, 6, 64);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				Close(false);
				return;
			}
			var internalJournalItem = new InternalJournalItem(Device, sendResult.Bytes);
			TotalCount = internalJournalItem.GKNo;
		}

		int _totalCount;
		public int TotalCount
		{
			get { return _totalCount; }
			set
			{
				_totalCount = value;
				OnPropertyChanged("TotalCount");
			}
		}

		int _startIndex;
		public int StartIndex
		{
			get { return _startIndex; }
			set
			{
				_startIndex = value;
				OnPropertyChanged("StartIndex");
			}
		}

		int _endIndex;
		public int EndIndex
		{
			get { return _endIndex; }
			set
			{
				_endIndex = value;
				OnPropertyChanged("EndIndex");
			}
		}

		int _lastCount = 0;
		public int LastCount
		{
			get { return _lastCount; }
			set
			{
				_lastCount = value;
				OnPropertyChanged("LastCount");
				StartIndex = Math.Max(0, TotalCount - value);
				EndIndex = TotalCount;
			}
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (StartIndex > EndIndex)
				return;

			JournalItems.Clear();
			LoadingService.Show("Запрос параметра", 2 + EndIndex - StartIndex);
			for (int i = StartIndex; i <= EndIndex; i++)
			{
				var data = new List<byte>();
				data = BitConverter.GetBytes(i).ToList();
				LoadingService.DoStep("Чтение записи " + i);
				var sendResult = SendManager.Send(Device, 4, 7, 64, data);
				if (sendResult.HasError)
				{
					MessageBoxService.Show("Ошибка связи с устройством");
					break;
				}
				var internalJournalItem = new InternalJournalItem(Device, sendResult.Bytes);
				var journalItem = internalJournalItem.ToJournalItem();
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}
			LoadingService.Close();
		}
	}
}