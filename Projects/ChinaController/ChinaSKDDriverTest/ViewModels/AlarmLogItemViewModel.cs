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
		private int _logNo;
		public int LogNo
		{
			get { return _logNo; }
			set
			{
				if (_logNo == value)
					return;
				_logNo = value;
				OnPropertyChanged(() => LogNo);
			}
		}

		private DateTime _dateTime;
		public DateTime DateTime
		{
			get { return _dateTime; }
			set
			{
				if (_dateTime == value)
					return;
				_dateTime = value;
				OnPropertyChanged(() => DateTime);
			}
		}

		private string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				if (_userName == value)
					return;
				_userName = value;
				OnPropertyChanged(() => UserName);
			}
		}

		private string _logType;
		public string LogType
		{
			get { return _logType; }
			set
			{
				if (_logType == value)
					return;
				_logType = value;
				OnPropertyChanged(() => LogType);
			}
		}

		private string _logMessage;
		public string LogMessage
		{
			get { return _logMessage; }
			set
			{
				if (_logMessage == value)
					return;
				_logMessage = value;
				OnPropertyChanged(() => LogMessage);
			}
		}

		private string _cardId;
		public string CardId
		{
			get { return _cardId; }
			set
			{
				if (_cardId == value)
					return;
				_cardId = value;
				OnPropertyChanged(() => CardId);
			}
		}

		private int _doorNo;
		public int DoorNo
		{
			get { return _doorNo; }
			set
			{
				if (_doorNo == value)
					return;
				_doorNo = value;
				OnPropertyChanged(() => DoorNo);
			}
		}

		private AlarmType _alarmType;
		public AlarmType AlarmType
		{
			get { return _alarmType; }
			set
			{
				if (_alarmType == value)
					return;
				_alarmType = value;
				OnPropertyChanged(() => AlarmType);
			}
		}

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
			//var journalItem = new SKDJournalItem();

			//journalItem.LoginID = MainViewModel.Wrapper.LoginID;
			//journalItem.SystemDateTime = DateTime.Now;
			//journalItem.DeviceDateTime = DateTime;
			//switch (AlarmType)
			//{
			//	case AlarmType.BreakIn:
			//		journalItem.JournalEventNameType = JournalEventNameType.Взлом;
			//		break;
			//	case AlarmType.Duress:
			//		journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
			//		break;
			//	case AlarmType.AlarmLocal:
			//		journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога;
			//		break;
			//	case AlarmType.DoorNotClose:
			//		journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта;
			//		break;
			//	case AlarmType.ReaderChassisIntruded:
			//		journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера;
			//		break;
			//	case AlarmType.Repeatenter:
			//		journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
			//		break;
			//}
			//journalItem.CardNo = CardId;
			//journalItem.DoorNo = DoorNo;

			var journalItem = MainViewModel.Wrapper.AlarmLogItemToJournalItem(new AlarmLogItem()
			{
				LogNo = LogNo,
				DateTime = DateTime,
				UserName = UserName,
				LogType = LogType,
				LogMessage = LogMessage,
				CardId = CardId,
				DoorNo = DoorNo,
				AlarmType = AlarmType
			});

			return journalItem;
		}
	}
}
