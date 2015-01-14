using System;
using DiagnosticsModule.Models;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		int Count = 0;

		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
			AddManyJournalCommand = new RelayCommand(OnAddManyJournal);
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

		public RelayCommand AddManyJournalCommand { get; private set; }
		void OnAddManyJournal()
		{
			
			var subsystemTypes = Enum.GetValues(typeof(JournalSubsystemType));
			var nameTypes = Enum.GetValues(typeof(JournalEventNameType));
			var descriptionTypes = Enum.GetValues(typeof(JournalEventDescriptionType));
			var objectTypes = Enum.GetValues(typeof(JournalObjectType));
			var rnd = new Random();
			for (int i = 0; i < 10000; i++)
			{
				var journalItem = new JournalItem();
				journalItem.DeviceDateTime = DateTime.Now;
				journalItem.JournalSubsystemType = (JournalSubsystemType)subsystemTypes.GetValue(rnd.Next(subsystemTypes.Length));
				journalItem.JournalEventNameType = (JournalEventNameType)nameTypes.GetValue(rnd.Next(nameTypes.Length));
				journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)descriptionTypes.GetValue(rnd.Next(descriptionTypes.Length));
				journalItem.JournalObjectType = (JournalObjectType)objectTypes.GetValue(rnd.Next(objectTypes.Length));
				FiresecManager.FiresecService.AddJournalItem(journalItem);
			}
			
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