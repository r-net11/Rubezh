﻿using System;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class OfflineLogItemViewModel : BaseViewModel
	{
		public int LoginID { get; private set; }

		public DateTime SystemDateTime { get; private set; }

		public DateTime? DeviceDateTime { get; private set; }

		public JournalEventNameType JournalEventNameType { get; private set; }

		public string Description { get; private set; }

		public string CardNo { get; private set; }

		public int DoorNo { get; private set; }

		public NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE EventType { get; private set; }

		public bool Status { get; private set; }
	
		public NativeWrapper.NET_ACCESSCTLCARD_TYPE CardType { get; private set; }
		
		public NativeWrapper.NET_ACCESS_DOOROPEN_METHOD OpenMethod { get; private set; }
		
		public string Pwd { get; private set; }
		
		public int Action { get; private set; }
		
		public NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE StatusType { get; private set; }
		
		public string ReaderID { get; private set; }
		
		public string DoorName { get; private set; } 

		public ErrorCode ErrorCode { get; set; }

		public OfflineLogItemViewModel(SKDJournalItem journalItem)
		{
			LoginID = journalItem.LoginID;
			SystemDateTime = journalItem.SystemDateTime;
			DeviceDateTime = journalItem.DeviceDateTime;
			JournalEventNameType = journalItem.JournalEventNameType;
			Description = journalItem.Description;
			CardNo = journalItem.CardNo;
			DoorNo = journalItem.DoorNo;
			EventType = journalItem.emEventType;
			Status = journalItem.bStatus;
			CardType = journalItem.emCardType;
			OpenMethod = journalItem.emOpenMethod;
			Pwd = journalItem.szPwd;
			Action = journalItem.nAction;
			StatusType = journalItem.emStatus;
			ReaderID = journalItem.szReaderID;
			DoorName = journalItem.szDoorName;
			ErrorCode = journalItem.ErrorCode;
		}
	}
}