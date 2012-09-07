using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.Database;

namespace EventsTest
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			JournalItems = new ObservableCollection<string>();
			DataContext = this;
		}

		public ObservableCollection<string> JournalItems { get; private set; }
		FiresecSerializedClient FiresecSerializedClient;
		int count = 1;

		void AddJournalItem(JournalRecord journalRecord)
		{
			var value = count.ToString() + "\t" + DateTime.Now.TimeOfDay.ToString() + "\t" + journalRecord.SystemTime.ToString() + "\t" + journalRecord.Description;
			JournalItems.Insert(0, value);
			count++;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			FiresecSerializedClient = new FiresecSerializedClient();
			var result = FiresecSerializedClient.Connect("adm", "").Result;
			JournalItems.Add(result ? "Соединение установлено" : "Ошибка при установлении соединения");
			SetLastEvent();
			FiresecSerializedClient.NewEvent += new Action<int>(FiresecClient_NewEvent);
		}

		void FiresecClient_NewEvent(int EventMask)
		{
			bool evmNewEvents = ((EventMask & 1) == 1);

			if (evmNewEvents)
				OnNewEvent();
		}

		void OnNewEvent()
		{
			var journalRecords = GetEventsFromLastId(LastEventId);

			foreach (var journalRecord in journalRecords)
			{
				AddJournalItem(journalRecord);
			}
		}

		int LastEventId = 0;
		void SetLastEvent()
		{
			Firesec.Journals.document journal = FiresecSerializedClient.ReadEvents(0, 100).Result;
			if (journal != null && journal.Journal != null)
			{
				foreach (var journalItem in journal.Journal)
				{
					var intValue = int.Parse(journalItem.IDEvents);
					if (intValue > LastEventId)
						LastEventId = intValue;
				}
			}
		}

		List<JournalRecord> GetEventsFromLastId(int lastId)
		{
			var result = new List<JournalRecord>();

			var hasNewRecords = true;
			while (hasNewRecords)
			{
				hasNewRecords = false;
				var document = FiresecSerializedClient.ReadEvents(LastEventId, 100).Result;
				if (document != null && document.Journal != null)
				{
					foreach (var innerJournalItem in document.Journal)
					{
						var eventId = int.Parse(innerJournalItem.IDEvents);
						if (eventId > LastEventId)
						{
							LastEventId = eventId;
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							result.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
			}

			return result;
		}
	}
}