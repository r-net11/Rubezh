using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		public static int AddCardRec(int loginID, CardRec cardRec)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC();
			stuCardRec.szCardNo = StringToCharArray(cardRec.CardNo, 32);
			stuCardRec.szPwd = StringToCharArray(cardRec.Password, 64);

			stuCardRec.stuTime.dwYear = cardRec.DateTime.Year;
			stuCardRec.stuTime.dwMonth = cardRec.DateTime.Month;
			stuCardRec.stuTime.dwDay = cardRec.DateTime.Day;
			stuCardRec.stuTime.dwHour = cardRec.DateTime.Hour;
			stuCardRec.stuTime.dwMinute = cardRec.DateTime.Minute;
			stuCardRec.stuTime.dwSecond = cardRec.DateTime.Second;

			stuCardRec.bStatus = cardRec.IsStatus;
			stuCardRec.emMethod = cardRec.DoorOpenMethod;
			stuCardRec.nDoor = cardRec.DoorNo;
			var result = NativeWrapper.WRAP_Insert_CardRec(loginID, ref stuCardRec);
			return result;
		}

		public static CardRec GetCardRecInfo(int loginID, int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetCardRecInfo(loginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC sdkCardRec = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cardRec = new CardRec();
			cardRec.RecordNo = sdkCardRec.nRecNo;
			cardRec.CardNo = CharArrayToString(sdkCardRec.szCardNo);
			cardRec.Password = CharArrayToString(sdkCardRec.szPwd);
			cardRec.DateTime = NET_TIMEToDateTime(sdkCardRec.stuTime);
			cardRec.IsStatus = sdkCardRec.bStatus;
			cardRec.DoorOpenMethod = sdkCardRec.emMethod;
			cardRec.DoorNo = sdkCardRec.nDoor;

			return cardRec;
		}

		public static List<CardRec> GetAllCardRecs(int loginID)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.CardRecsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAllCardRecs(loginID, intPtr);

			NativeWrapper.CardRecsCollection cardRecsCollection = (NativeWrapper.CardRecsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardRecsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cardRecs = new List<CardRec>();

			for (int i = 0; i < Math.Min(cardRecsCollection.Count, 500); i++)
			{
				var sdkCard = cardRecsCollection.CardRecs[i];
				var card = new CardRec();
				card.RecordNo = sdkCard.nRecNo;
				card.DateTime = NET_TIMEToDateTime(sdkCard.stuTime);
				card.CardNo = CharArrayToString(sdkCard.szCardNo);
				card.Password = CharArrayToString(sdkCard.szPwd);
				card.DoorNo = sdkCard.nDoor;
				card.IsStatus = sdkCard.bStatus;
				card.DoorOpenMethod = sdkCard.emMethod;
				cardRecs.Add(card);
			}
			return cardRecs;
		}
	}
}