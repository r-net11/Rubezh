using System;
using Localization.Journal.ViewModels;
using StrazhAPI.Journal;
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
			Title = CommonViewModels.AcceptCriticalEvent;
			ConfirmCommand = new RelayCommand(OnConfirm);
			JournalItemViewModel = new JournalItemViewModel(journalItem);
		}

		public JournalItemViewModel JournalItemViewModel { get; private set; }

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			Close();
		}

		public override void OnClosed()
		{
			var deltaSeconds = (int)(DateTime.Now - StartDateTime).TotalSeconds;
			var journalItem = new JournalItem();
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Подтверждение_тревоги;
		    journalItem.DescriptionText = string.Format(CommonViewModels.ReactionTime, JournalItemViewModel.Name, JournalItemViewModel.Description, deltaSeconds);
			FiresecManager.FiresecService.AddJournalItem(journalItem);
		}
	}
}