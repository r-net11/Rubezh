using System;
using StrazhDeviceSDK.API;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessLogItemViewModel : BaseViewModel
	{
		private int _recordNo;
		public int RecordNo
		{
			get { return _recordNo; }
			set
			{
				if (_recordNo == value)
					return;
				_recordNo = value;
				OnPropertyChanged(() => RecordNo);
			}
		}

		private string _cardNo;
		public string CardNo
		{
			get { return _cardNo; }
			set
			{
				if (_cardNo == value)
					return;
				_cardNo = value;
				OnPropertyChanged(() => CardNo);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				if (_password == value)
					return;
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		private DateTime _time;
		public DateTime Time
		{
			get { return _time; }
			set
			{
				if (_time == value)
					return;
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		private bool _status;
		public bool Status
		{
			get { return _status; }
			set
			{
				if (_status == value)
					return;
				_status = value;
				OnPropertyChanged(() => Status);
			}
		}

		private AccessMethodType _methodType;
		public AccessMethodType MethodType
		{
			get { return _methodType; }
			set
			{
				if (_methodType == value)
					return;
				_methodType = value;
				OnPropertyChanged(() => MethodType);
			}
		}

		private string _readerID;
		public string ReaderID
		{
			get { return _readerID; }
			set
			{
				if (_readerID == value)
					return;
				_readerID = value;
				OnPropertyChanged(() => ReaderID);
			}
		}

		private CardType _cardType;
		public CardType CardType
		{
			get { return _cardType; }
			set
			{
				if (_cardType == value)
					return;
				_cardType = value;
				OnPropertyChanged(() => CardType);
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

		private ErrorCode _errorCode;
		public ErrorCode ErrorCode
		{
			get { return _errorCode; }
			set
			{
				if (_errorCode == value)
					return;
				_errorCode = value;
				OnPropertyChanged(() => ErrorCode);
			}
		}

		public AccessLogItemViewModel(AccessLogItem accessLogItem)
		{
			RecordNo = accessLogItem.RecordNo;
			CardNo = accessLogItem.CardNo;
			Password = accessLogItem.Password;
			Time = accessLogItem.Time;
			Status = accessLogItem.Status;
			MethodType = accessLogItem.MethodType;
			ReaderID = accessLogItem.ReaderID;
			CardType = accessLogItem.CardType;
			DoorNo = accessLogItem.DoorNo;
			ErrorCode = accessLogItem.ErrorCode;
		}

		public SKDJournalItem TransformToJournalItem()
		{
			//var journalItem = new SKDJournalItem();

			//journalItem.LoginID = MainViewModel.Wrapper.LoginID;
			//journalItem.SystemDateTime = DateTime.Now;
			//journalItem.DeviceDateTime = Time;
			//journalItem.JournalEventNameType = Status
			//	? JournalEventNameType.Проход_разрешен
			//	: JournalEventNameType.Проход_запрещен;
			//journalItem.CardNo = CardNo;
			//journalItem.DoorNo = DoorNo;
			//journalItem.bStatus = Status;
			//journalItem.emCardType = (NativeWrapper.NET_ACCESSCTLCARD_TYPE) CardType;
			//journalItem.emOpenMethod = (NativeWrapper.NET_ACCESS_DOOROPEN_METHOD) MethodType;
			//journalItem.szPwd = Password;
			////journalItem.nAction = 
			////journalItem.emStatus = 
			//journalItem.szReaderID = (ReaderID + 1).ToString();
			////journalItem.szDoorName = 
			//journalItem.ErrorCode = ErrorCode;

			var journalItem = MainViewModel.Wrapper.AccessLogItemToJournalItem(new AccessLogItem()
			{
				RecordNo = RecordNo,
				CardNo = CardNo,
				Password = Password,
				Time = Time,
				Status = Status,
				MethodType = MethodType,
				ReaderID = ReaderID,
				DoorNo = DoorNo,
				CardType = CardType,
				ErrorCode = ErrorCode
			});

			return journalItem;
		}
	}
}