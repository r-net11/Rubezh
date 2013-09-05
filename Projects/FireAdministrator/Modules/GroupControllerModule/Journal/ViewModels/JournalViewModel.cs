using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization;
using Common.GK.Journal;
using FiresecClient;


namespace GKModule.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		XDevice Device;

		public JournalViewModel(XDevice device)
		{
			Title = "Журнал событий ГК";
			ReadCommand = new RelayCommand(OnRead);
			SaveToFileCommand = new RelayCommand(OnSaveToFile);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Device = device;
		}

		public bool Initialize()
		{
			var sendResult = SendManager.Send(Device, 0, 6, 64);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				return false;
			}
			var internalJournalItem = new InternalJournalItem(Device, sendResult.Bytes);
			TotalCount = internalJournalItem.GKNo;
			StartIndex = Math.Max(1, TotalCount - 100);
			EndIndex = TotalCount;
			return true;
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

		bool _isNotEmpty;
		public bool IsNotEmpty
		{
			get { return _isNotEmpty; }
			set
			{
				_isNotEmpty = value;
				OnPropertyChanged("IsNotEmpty");
			}
			
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (StartIndex > EndIndex)
			{
				IsNotEmpty = false;
				return;
			}

			JournalItems.Clear();
			LoadingService.Show("Чтение записей журнала", 2 + EndIndex - StartIndex);
			for (int i = StartIndex; i <= EndIndex; i++)
			{
				var data = BitConverter.GetBytes(i).ToList();
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
			IsNotEmpty = true;
			LoadingService.Close();
		}

		public RelayCommand SaveToFileCommand { get; private set; }
		void OnSaveToFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Журнал событий Firesec-2|*.fscj",
				DefaultExt = "Журнал событий Firesec-2|*.fscj"
			};
			if (saveDialog.ShowDialog().Value)
			{
				WaitHelper.Execute(() =>
				{
					if (File.Exists(saveDialog.FileName))
						File.Delete(saveDialog.FileName);

					var dataContractSerializer = new DataContractSerializer(typeof(JournalItemsCollection));
					using (var fileStream = new FileStream(saveDialog.FileName, FileMode.CreateNew))
					{
						var journalItems = new System.Collections.Generic.List<JournalItem>();
						JournalItems.ToList().ForEach(x => journalItems.Add(x.JournalItem));
						var journalItemsCollection = new JournalItemsCollection
						{
							JournalItems = journalItems,
							RecordCount = TotalCount,
							CreationDateTime = DateTime.Now,
						};
						dataContractSerializer.WriteObject(fileStream, journalItemsCollection);
					}
				});
			}
		}
	}
}