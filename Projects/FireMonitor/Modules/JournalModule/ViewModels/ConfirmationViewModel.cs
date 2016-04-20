using System;
using RubezhAPI.Journal;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows.Windows;
using System.Windows;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		static Window parentWindow;
		DateTime StartDateTime = DateTime.Now;

		public ConfirmationViewModel(JournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
			JournalItemViewModel = new JournalItemViewModel(journalItem);

			if (parentWindow == null)
				parentWindow = DialogService.GetActiveWindow();
        }

		public JournalItemViewModel JournalItemViewModel { get; private set; }

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			Close();
		}
		public override void OnLoad()
		{
			Surface.Owner = parentWindow;
			Surface.ShowInTaskbar = Surface.Owner == null;
			Surface.WindowStartupLocation = Surface.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
		}
		public override void OnClosed()
		{
			var deltaSeconds = (int)(DateTime.Now - StartDateTime).TotalSeconds;
			var journalItem = new JournalItem();
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Подтверждение_тревоги;
			journalItem.DescriptionText = JournalItemViewModel.Name + " " + JournalItemViewModel.Description + " (время реакции " + deltaSeconds.ToString() + " сек)";
			ClientManager.FiresecService.AddJournalItem(journalItem);
			AlarmPlayerHelper.Stop();
		}
	}
}