using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FS2Api;
using Infrastructure;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
        public ConfirmationViewModel(JournalRecord journalRecord)
		{
			Title = "Подтверждение критических событий";
            ConfirmCommand = new RelayCommand(OnConfirm);
			JournalRecord = journalRecord;
			ZoneName = journalRecord.ZoneName;
			StateType = journalRecord.StateType;
		}

		public ConfirmationViewModel(FS2JournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
			ZoneName = journalItem.ZoneName;
			StateType = journalItem.StateType;
		}

        public JournalRecord JournalRecord { get; private set; }
		public string ZoneName { get; private set; }
		public StateType StateType { get; private set; }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
            var journalRecords = new List<JournalRecord>();
            journalRecords.Add(
                new JournalRecord()
                {
                    SystemTime = DateTime.Now,
                    DeviceTime = DateTime.Now,
                    ZoneName = ZoneName,
                    Description = "Состояние \"" + StateType.ToDescription() + "\" подтверждено оператором",
                    StateType = StateType.Info
                }
                );
            FiresecManager.FiresecService.AddJournalRecords(journalRecords);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Publish(journalRecords);
            Close();
        }
	}
}