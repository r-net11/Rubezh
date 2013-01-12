using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		public ConfirmationViewModel(JournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
            JournalItem = journalItem;
		}

		public JournalItem JournalItem { get; private set; }

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			GKDBHelper.AddMessage("Состояние \"" + JournalItem.Name + " " + JournalItem.Description + "\" подтверждено оператором");
			Close();
		}
	}
}