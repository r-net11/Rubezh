using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure;
using GKModule.Events;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
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
			var journalItem = GKDBHelper.AddMessage("Состояние \"" + JournalItemViewModel.JournalItem.Name + " " + JournalItemViewModel.JournalItem.Description + "\" подтверждено оператором");
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(new List<JournalItem>() { journalItem });
			Close();
		}
	}
}