using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AlarmLogItemViewModel : BaseViewModel
	{
		public int LogNo { get; set; }

		public DateTime DateTime { get; set; }

		public string UserName { get; set; }

		public string LogType { get; set; }

		public string LogMessage { get; set; }

		public string CardId { get; set; }

		public int DoorNo { get; set; }

		public AlarmType AlarmType { get; set; }

		public AlarmLogItemViewModel(AlarmLogItem alarmLogItem)
		{
			LogNo = alarmLogItem.LogNo;
			DateTime = alarmLogItem.DateTime;
			UserName = alarmLogItem.UserName;
			LogType = alarmLogItem.LogType;
			LogMessage = alarmLogItem.LogMessage;
			CardId = alarmLogItem.CardId;
			DoorNo = alarmLogItem.DoorNo;
			AlarmType = alarmLogItem.AlarmType;
		}

		public SKDJournalItem TransformToJournalItem()
		{
			var journalItem = new SKDJournalItem();

			journalItem.LoginID = MainViewModel.Wrapper.LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime;
			switch (AlarmType)
			{
				case AlarmType.BreakIn:
					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					break;
				case AlarmType.Duress:
					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					break;
				case AlarmType.AlarmLocal:
					journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога;
					break;
				case AlarmType.DoorNotClose:
					journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта;
					break;
				case AlarmType.ReaderChassisIntruded:
					journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера;
					break;
				case AlarmType.Repeatenter:
					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					break;
			}
			journalItem.CardNo = CardId;
			journalItem.DoorNo = DoorNo;

			return journalItem;
		}
	}
}
