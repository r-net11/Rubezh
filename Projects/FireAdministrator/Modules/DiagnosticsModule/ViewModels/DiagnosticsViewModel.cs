using System;
using System.IO;
using DiagnosticsModule.Models;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Ionic.Zip;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		int Count = 0;

		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
			SaveCommand = new RelayCommand(OnSave);
			LoadCommand = new RelayCommand(OnLoad);

		}

		public RelayCommand AddJournalCommand { get; private set; }
		void OnAddJournal()
		{
			var journalItem = new JournalItem();
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Подтверждение_тревоги;
			FiresecManager.FiresecService.AddJournalItem(journalItem);
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			SerializerHelper.Save();
		}

		public RelayCommand LoadCommand { get; private set; }
		void OnLoad()
		{
			SerializerHelper.Load();
		}


		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}
	}
}