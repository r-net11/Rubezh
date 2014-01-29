using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure;
using GKModule.Events;
using System.Collections.Generic;
using System;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
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
			FiresecManager.FiresecService.GKAddMessage(EventNameEnum.Подтверждение_тревоги,
				JournalItemViewModel.JournalItem.Name + " " + JournalItemViewModel.JournalItem.Description +
				" (время реакции " + deltaSeconds.ToString() + " сек)");
			Close();
		}
	}
}