using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		public JournalViewModel()
		{
			Title = "Журнал событий ГК";
			ReadCommand = new RelayCommand(OnRead);
			JournalItems = new ObservableCollection<JournalItem>();
			StartIndex = 1;
			EndIndex = 100;
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
			for (int i = StartIndex; i <= EndIndex; i++)
			{
				var data = new List<byte>();
				data = BitConverter.GetBytes(i).ToList();
				var bytes = CommandManager.Send(4, 7, 64, data);
				var journalItem = new JournalItem(bytes);
				JournalItems.Add(journalItem);
			}
		}
	}
}