using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Common.GK;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		public ConfirmationViewModel(JournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
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