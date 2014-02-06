using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
        public ConfirmationViewModel(JournalRecord journalRecord)
		{
			Title = "Подтверждение критических событий";
            ConfirmCommand = new RelayCommand(OnConfirm);
			JournalRecord = new JournalRecordViewModel(journalRecord);
		}

		//public ConfirmationViewModel(FS2JournalItem journalItem)
		//{
		//    Title = "Подтверждение критических событий";
		//    ConfirmCommand = new RelayCommand(OnConfirm);
		//    JournalRecord = new JournalRecordViewModel(journalItem);
		//}

        public JournalRecordViewModel JournalRecord { get; private set; }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
			//if (FiresecManager.IsFS2Enabled)
			//{
			//    var journalItems = new List<FS2JournalItem>();
			//    journalItems.Add(
			//        new FS2JournalItem()
			//        {
			//            SystemTime = DateTime.Now,
			//            DeviceTime = DateTime.Now,
			//            ZoneName = JournalRecord.ZoneName,
			//            Description = "Состояние \"" + JournalRecord.StateType.ToDescription() + "\" подтверждено оператором",
			//            StateType = StateType.Info
			//        });
			//    FiresecManager.FS2ClientContract.AddJournalItems(journalItems);
			//    ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Publish(journalItems);
			//}
			//else
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
					});
				FiresecManager.FiresecService.AddJournalRecords(journalRecords);
				ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Publish(journalRecords);
			}
            Close();
        }
	}
}