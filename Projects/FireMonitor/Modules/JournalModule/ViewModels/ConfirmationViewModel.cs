using System;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		DateTime StartDateTime = DateTime.Now;

		public ConfirmationViewModel(JournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
			JournalItemViewModel = new JournalItemViewModel(journalItem);
		}

		public JournalItemViewModel JournalItemViewModel { get; private set; }

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			var deltaSeconds = (int)(DateTime.Now - StartDateTime).TotalSeconds;
			//FiresecManager.FiresecService.AddJournalItem(JournalEventNameType.Подтверждение_тревоги,
			//    JournalItemViewModel.Name + " " + JournalItemViewModel.Description +
			//    " (время реакции " + deltaSeconds.ToString() + " сек)");
			Close();
		}
	}
}