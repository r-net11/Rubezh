using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		XDevice Device;

		public JournalViewModel(XDevice device)
		{
			Title = "Журнал событий ГК " + device.GetGKIpAddress();
			ReadCommand = new RelayCommand(OnRead);
			SaveToFileCommand = new RelayCommand(OnSaveToFile);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Device = device;

			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			DescriptorsManager.CreateDynamicObjectsInXManager();
		}

		public bool Initialize()
		{
			var result = FiresecManager.FiresecService.GKGetJournalItemsCount(Device);
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return false;
			}
			TotalCount = result.Result;
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
			try
			{
				LoadingService.Show("Чтение записей журнала", "Чтение записей журнала", 2 + EndIndex - StartIndex, true);
				for (int i = StartIndex; i <= EndIndex; i++)
				{
					if (LoadingService.IsCanceled)
						break;
					LoadingService.DoStep("Чтение записи " + i);
					var result = FiresecManager.FiresecService.GKReadJournalItem(Device, i);
					if (result.HasError)
					{
						MessageBoxService.Show(result.Error);
						break;
					}
					var journalItemViewModel = new JournalItemViewModel(result.Result);
					JournalItems.Add(journalItemViewModel);
				}
				IsNotEmpty = true;
			}
			finally
			{
				LoadingService.Close();
			}
		}

		public RelayCommand SaveToFileCommand { get; private set; }
		void OnSaveToFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Журнал событий Firesec|*.fscj",
				DefaultExt = "Журнал событий Firesec|*.fscj"
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
							GkIP = Device.GetGKIpAddress()
						};
						dataContractSerializer.WriteObject(fileStream, journalItemsCollection);
					}
				});
			}
		}
	}
}