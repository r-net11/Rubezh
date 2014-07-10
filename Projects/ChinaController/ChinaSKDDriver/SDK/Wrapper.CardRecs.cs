using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddCardRec(CardRec cardRec)
		{
			var nativeCardRec = CardRecToNativeCardRec(cardRec);
			var result = NativeWrapper.WRAP_Insert_CardRec(LoginID, ref nativeCardRec);
			return result;
		}

		public bool EditCardRec(CardRec cardRec)
		{
			var nativeCardRec = CardRecToNativeCardRec(cardRec);
			var result = NativeWrapper.WRAP_Update_CardRec(LoginID, ref nativeCardRec);
			return result;
		}

		public CardRec GetCardRecInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_CardRec_Info(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cardRec = NativeCardRecToCardRec(nativeCardRec);
			return cardRec;
		}

		public bool RemoveCardRec(int index)
		{
			var result = NativeWrapper.WRAP_Remove_CardRec(LoginID, index);
			return result;
		}

		public bool RemoveAllCardRecs()
		{
			var result = NativeWrapper.WRAP_RemoveAll_CardRecs(LoginID);
			return result;
		}

		public int GetCardRecsCount()
		{
			var cardsCount = NativeWrapper.WRAP_Get_CardRecs_Count(LoginID);
			return cardsCount;
		}

		public List<CardRec> GetAllCardRecs()
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.CardRecsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAll_CardRecs(LoginID, intPtr);

			NativeWrapper.CardRecsCollection cardRecsCollection = (NativeWrapper.CardRecsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CardRecsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var cardRecs = new List<CardRec>();

			for (int i = 0; i < Math.Min(cardRecsCollection.Count, 10); i++)
			{
				var sdkCard = cardRecsCollection.CardRecs[i];
				var card = new CardRec();
				card.RecordNo = sdkCard.nRecNo;
				card.DateTime = NET_TIMEToDateTime(sdkCard.stuTime);
				card.CardNo = CharArrayToString(sdkCard.szCardNo);
				card.Password = CharArrayToString(sdkCard.szPwd);
				card.DoorNo = sdkCard.nDoor;
				card.IsStatus = sdkCard.bStatus;
				card.DoorOpenMethod = (CardRecDoorOpenMethod)sdkCard.emMethod;
				cardRecs.Add(card);
			}
			return cardRecs;
		}

		NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC CardRecToNativeCardRec(CardRec cardRec)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC();
			nativeCardRec.szCardNo = StringToCharArray(cardRec.CardNo, 32);
			nativeCardRec.szPwd = StringToCharArray(cardRec.Password, 64);

			nativeCardRec.stuTime.dwYear = cardRec.DateTime.Year;
			nativeCardRec.stuTime.dwMonth = cardRec.DateTime.Month;
			nativeCardRec.stuTime.dwDay = cardRec.DateTime.Day;
			nativeCardRec.stuTime.dwHour = cardRec.DateTime.Hour;
			nativeCardRec.stuTime.dwMinute = cardRec.DateTime.Minute;
			nativeCardRec.stuTime.dwSecond = cardRec.DateTime.Second;

			nativeCardRec.bStatus = cardRec.IsStatus;
			nativeCardRec.emMethod = (ChinaSKDDriverNativeApi.NativeWrapper.NET_ACCESS_DOOROPEN_METHOD)cardRec.DoorOpenMethod;
			nativeCardRec.nDoor = cardRec.DoorNo;
			return nativeCardRec;
		}

		CardRec NativeCardRecToCardRec(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec)
		{
			var cardRec = new CardRec();
			cardRec.RecordNo = nativeCardRec.nRecNo;
			cardRec.CardNo = CharArrayToString(nativeCardRec.szCardNo);
			cardRec.Password = CharArrayToString(nativeCardRec.szPwd);
			cardRec.DateTime = NET_TIMEToDateTime(nativeCardRec.stuTime);
			cardRec.IsStatus = nativeCardRec.bStatus;
			cardRec.DoorOpenMethod = (CardRecDoorOpenMethod)nativeCardRec.emMethod;
			cardRec.DoorNo = nativeCardRec.nDoor;
			return cardRec;
		}
	}
}