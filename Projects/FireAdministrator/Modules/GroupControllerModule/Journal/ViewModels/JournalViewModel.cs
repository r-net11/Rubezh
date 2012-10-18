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
			JournalItems = new ObservableCollection<JournalItem>();
			Device = device;
			StartIndex = 1;
			EndIndex = 100;
			SetTotalCount();
		}

		void SetTotalCount()
		{
			var sendResult = SendManager.Send(Device, 0, 6, 64);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				return;
			}
			var journalItem = new JournalItem(Device, sendResult.Bytes);
			TotalCount = journalItem.GKNo;
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

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (StartIndex > EndIndex)
				return;

			JournalItems.Clear();
			LoadingService.Show("Запрос параметра", 2 + EndIndex - StartIndex);
            //var gke = new GkEvents();
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
				var journalItem = new JournalItem(Device, sendResult.Bytes);
				JournalItems.Add(journalItem);
                //gke.Add(journalItem);
			}
			LoadingService.Close();
		}
	}
}