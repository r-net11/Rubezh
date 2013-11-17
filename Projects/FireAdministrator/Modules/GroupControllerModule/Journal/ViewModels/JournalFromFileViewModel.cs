using System.Collections.ObjectModel;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class JournalFromFileViewModel : DialogViewModel
	{
		public JournalFromFileViewModel(JournalItemsCollection journalItemsCollection)
		{
			Title = "Журнал событий ГК";
			DescriptorsManager.Create();
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			journalItemsCollection.JournalItems.ForEach(x => JournalItems.Add(new JournalItemViewModel(x)));
			CreationDateTimeString = "Файл создан " + journalItemsCollection.CreationDateTime.ToString();
			RecordsCountString = "Записей в журнале прибора " + journalItemsCollection.RecordCount.ToString();
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

		string creationDateTimeString;
		public string CreationDateTimeString
		{
			get { return creationDateTimeString; }
			set
			{
				creationDateTimeString = value;
				OnPropertyChanged("CreationDateTimeString");
			}
		}

		string recordsCountString;
		public string RecordsCountString
		{
			get { return recordsCountString; }
			set
			{
				recordsCountString = value;
				OnPropertyChanged("RecordsCountString");
			}
		}
	}
}