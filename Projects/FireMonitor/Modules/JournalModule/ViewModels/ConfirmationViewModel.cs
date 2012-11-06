using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
        public ConfirmationViewModel(JournalRecord journalRecord)
		{
			Title = "Подтверждение критических событий";
            ConfirmCommand = new RelayCommand(OnConfirm);
			JournalRecord = journalRecord;
		}

        public JournalRecord JournalRecord { get; private set; }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
            var journalRecords = new List<JournalRecord>();
            journalRecords.Add(
                new JournalRecord()
                {
                    SystemTime = DateTime.Now,
                    DeviceTime = DateTime.Now,
                    ZoneName = JournalRecord.ZoneName,
                    Description = "Состояние \"" + JournalRecord.StateType.ToDescription() + "\" подтверждено оператором",
                    StateType = StateType.Info
                }
                );
            FiresecManager.FiresecService.AddJournalRecords(journalRecords);

            Close();
        }
	}
}