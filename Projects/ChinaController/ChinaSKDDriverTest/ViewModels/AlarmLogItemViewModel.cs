using System;
using Infrastructure.Common.Windows.ViewModels;
using StrazhDeviceSDK;
using StrazhDeviceSDK.API;

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

		private int _channel;
		public int Channel
		{
			get { return _channel; }
			set
			{
				if (_channel == value)
					return;
				_channel = value;
				OnPropertyChanged(() => Channel);
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
			Channel = alarmLogItem.Channel;
			AlarmType = alarmLogItem.AlarmType;
		}

		public SKDJournalItem TransformToJournalItem()
		{
			var journalItem = MainViewModel.Wrapper.AlarmLogItemToJournalItem(new AlarmLogItem()
			{
				LogNo = LogNo,
				DateTime = DateTime,
				UserName = UserName,
				LogType = LogType,
				LogMessage = LogMessage,
				CardId = CardId,
				Channel = Channel,
				AlarmType = AlarmType
			});

			return journalItem;
		}
	}
}
