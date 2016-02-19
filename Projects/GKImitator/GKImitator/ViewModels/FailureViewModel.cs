using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Journal;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public class FailureViewModel : BaseViewModel
	{
		DescriptorViewModel BinaryObjectViewModel;
		public JournalEventDescriptionType JournalEventDescriptionType { get; private set; }
		public byte No { get; private set; }

		public FailureViewModel(DescriptorViewModel binaryObjectViewModel, JournalEventDescriptionType journalEventDescriptionType, byte no)
		{
			BinaryObjectViewModel = binaryObjectViewModel;
			JournalEventDescriptionType = journalEventDescriptionType;
			No = no;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);

				var journalItem = new ImitatorJournalItem(2, 5, No, (byte)(value ? 1 : 0));
				BinaryObjectViewModel.AddJournalItem(journalItem);
			}
		}
	}
}