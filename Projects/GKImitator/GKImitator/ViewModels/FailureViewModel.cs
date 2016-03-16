using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
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
				BinaryObjectViewModel.SetStateBit(GKStateBit.Failure, value);
			}
		}
	}
}