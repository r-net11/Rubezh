using System.Collections.ObjectModel;
using GKProcessor;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Journal;

namespace GKModule.ViewModels
{
	public class JournalFromFileViewModel : DialogViewModel
	{
		public JournalFromFileViewModel(JournalItemsCollection journalItemsCollection)
		{
			Title = "Журнал событий ГК";
			DescriptorsManager.Create();
			JournalItemsCollection = journalItemsCollection;
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			journalItemsCollection.JournalItems.ForEach(x => JournalItems.Add(new JournalItemViewModel(x)));
		}

		public JournalItemsCollection JournalItemsCollection { get; private set; }
		
		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged(() => JournalItems);
			}
		}
	}
}