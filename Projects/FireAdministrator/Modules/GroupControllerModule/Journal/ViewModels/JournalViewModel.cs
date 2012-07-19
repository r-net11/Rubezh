using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
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
			EraseCommand = new RelayCommand(OnErase);
			JournalItems = new ObservableCollection<JournalItem>();
			Device = device;
			StartIndex = 1;
			EndIndex = 100;
			SetTotalCount();
		}

		void SetTotalCount()
		{
			var bytes = SendManager.Send(Device, 0, 6, 64);
			var journalItem = new JournalItem(bytes);
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
			for (int i = StartIndex; i <= EndIndex; i++)
			{
				var data = new List<byte>();
				data = BitConverter.GetBytes(i).ToList();
				LoadingService.DoStep("Чтение записи " + i);
				var bytes = SendManager.Send(Device, 4, 7, 64, data);
				var journalItem = new JournalItem(bytes);
				JournalItems.Add(journalItem);
			}
			LoadingService.Close();
		}

		public RelayCommand EraseCommand { get; private set; }
		void OnErase()
		{
			SendManager.Send(Device, 0, 8, 0);
		}
	}
}