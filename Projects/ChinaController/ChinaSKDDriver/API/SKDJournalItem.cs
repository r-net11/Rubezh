using StrazhDeviceSDK.NativeAPI;
using FiresecAPI.Journal;
using System;

namespace StrazhDeviceSDK.API
{
	public class SKDJournalItem
	{
		public JournalItemType JournalItemType { get; set; }

		public int LoginID { get; set; }

		public DateTime SystemDateTime { get; set; }

		public DateTime? DeviceDateTime { get; set; }

		public JournalEventNameType JournalEventNameType { get; set; }

		public string Description { get; set; }

		public string CardNo { get; set; }

		public int DoorNo { get; set; }

		public NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE emEventType; // Направление прохода (вход/выход)
		public bool bStatus; // Проход разрешен/запрещен
		public NativeWrapper.NET_ACCESSCTLCARD_TYPE emCardType; // Тип пропуска
		public NativeWrapper.NET_ACCESS_DOOROPEN_METHOD emOpenMethod; // Метод открытия замка
		public string szPwd; // Пароль для метода открытия замка через ввод пароля
		public int nAction;
		public NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE emStatus;
		public string szReaderID; // Номер считывателя (начиная с 1)
		public string szDoorName;

		public ErrorCode ErrorCode { get; set; }
	}
}