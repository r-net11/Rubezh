using System;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessLogItemViewModel : BaseViewModel
	{
		public int RecordNo { get; set; }

		public string CardNo { get; set; }

		public string Password { get; set; }

		public DateTime Time { get; set; }

		public bool Status { get; set; }

		public AccessMethodType MethodType { get; set; }

		public int ReaderID { get; set; }

		public int DoorNo { get; set; }

		public AccessLogItemViewModel(AccessLogItem accessLogItem)
		{
			RecordNo = accessLogItem.RecordNo;
			CardNo = accessLogItem.CardNo;
			Password = accessLogItem.Password;
			Time = accessLogItem.Time;
			Status = accessLogItem.Status;
			MethodType = accessLogItem.MethodType;
			ReaderID = accessLogItem.ReaderID;
			DoorNo = accessLogItem.DoorNo;
		}

		public SKDJournalItem TransformToJournalItem()
		{
			var journalItem = new SKDJournalItem();

			journalItem.LoginID = MainViewModel.Wrapper.LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = Time;
			journalItem.JournalEventNameType = Status
				? JournalEventNameType.Проход_разрешен
				: JournalEventNameType.Проход_запрещен;
			journalItem.CardNo = CardNo;
			journalItem.DoorNo = DoorNo;
			journalItem.bStatus = Status;
			journalItem.emOpenMethod = (NativeWrapper.NET_ACCESS_DOOROPEN_METHOD) MethodType;
			journalItem.szReaderID = (ReaderID + 1).ToString();

			return journalItem;
		}
	}
}